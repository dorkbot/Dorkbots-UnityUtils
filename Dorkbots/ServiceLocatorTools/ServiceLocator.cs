/*
* Author: Dayvid jones
* http://www.dayvid.com
* Copyright (c) Superhero Robot 2018
* http://www.superherorobot.com
* Manged by Dorkbots
* http://www.dorkbots.com/
* Version: 1
*
* Licence Agreement
*
* You may distribute and modify this class freely, provided that you leave this header intact,
* and add appropriate headers to indicate your changes. Credit is appreciated in applications
* that use this code, but is not required.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.ServiceLocatorTools
{
    /// <summary>
    /// A static utility class to provide a simplistic service location mechanism for dependency resolution.
    /// This class is used in the Service Locator Pattern (read more -> https://en.wikipedia.org/wiki/Service_locator_pattern).
    /// The service locator pattern is a design pattern used in software development to encapsulate the processes involved in obtaining a service with a strong abstraction layer.
    /// This pattern uses a central registry known as the “service locator” which on request returns the information necessary to perform a certain task.
    /// </summary>
    public static class ServiceLocator
    {
        // Global registry for Reset()
        private static readonly List<Action> Resetters = new List<Action>();
        /*
         * Synchronize access to shared resources in a multithreaded environment.
         * Its primary purpose is to ensure that only one thread can execute a specific block of code, known as a critical section, at any given time.
         * This prevents race conditions and ensures data integrity when multiple threads are trying to modify the same data concurrently.
         */
        private static readonly object ResettersLock = new object();//This is an object that acts as the lock

        /// <summary>
        /// Registers a service with the specified instance, making it available for retrieval.
        /// </summary>
        /// <typeparam name="T">The type of the service being registered.</typeparam>
        /// <param name="instance">The instance of the service to register.</param>
        /// <returns>Returns the registered service instance.</returns>
        public static T Register<T>(T instance)
        {
            if (instance == null)
            {
                Debug.LogError($"Register instance for {typeof(T)} is null.");
                return default;
            }

            ServiceSlot<T>.EnsureRegisteredForReset();
            
            T result;
            lock (ServiceSlot<T>.SlotLock)
            {
                if (ServiceSlot<T>.HasInstance)
                {
                    Debug.LogError($"Service {typeof(T)} is already registered! Unregister it first and check for references to the previous instance.");
                    result = ServiceSlot<T>.Instance;
                }
                else
                {
                    ServiceSlot<T>.Instance = instance;
                    ServiceSlot<T>.HasInstance = true;
                    result = instance;
                }
            }

            // Always notify any queued requests with the current instance
            ServiceSlot<T>.Notify(result);
            return result;
        }

        // Strongly typed lazy registration via factory (avoids reflection)
        /// <summary>
        /// Registers a service lazily with a factory method to create the instance when required.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="factory">A factory function to create the service instance when needed.</param>
        public static void RegisterLazy<T>(Func<T> factory)
        {
            if (factory == null)
            {
                Debug.LogError($"RegisterLazy factory for {typeof(T)} is null.");
                return;
            }

            ServiceSlot<T>.EnsureRegisteredForReset();

            // Prevent lazy registration if an instance already exists for T or if a factory is already set
            lock (ServiceSlot<T>.SlotLock)
            {
                if (ServiceSlot<T>.HasInstance)
                {
                    Debug.LogError($"Service {typeof(T)} is already registered with an instance. RegisterLazy is not allowed. Use Unregister/UnregisterLazy first if you need to replace it.");
                    return;
                }

                if (ServiceSlot<T>.Factory != null)
                {
                    Debug.LogError($"Service {typeof(T)} already is already lazy registered. RegisterLazy is not allowed. Use UnregisterLazy first if you need to replace it.");
                    return;
                }

                ServiceSlot<T>.Factory = factory;
            }

            // If something already requested this service, resolve now so they get it immediately
            if (ServiceSlot<T>.HasPendingRequests)
                Resolve<T>();
        }

        /// <summary>
        /// Resolves and retrieves an instance of the specified service type.
        /// If the service has not been registered but a factory is defined for it, the factory is invoked to create and register the instance.
        /// </summary>
        /// <typeparam name="T">The type of the service to resolve.</typeparam>
        /// <returns>Returns the resolved instance of the specified service type.</returns>
        public static T Resolve<T>()
        {
            if (!ServiceSlot<T>.HasInstance)//don't have an instance, so T hasn't been registered, OR it's been lazy registered.
            {
                if (ServiceSlot<T>.Factory != null)
                {
                    // Create and register via the factory exactly once
                    Register(ServiceSlot<T>.Factory());
                }
                else
                {
                    Debug.LogError($"The service {typeof(T)} has not been registered. Call IsRegistered first to check. OR pass a callback function and then handle when its registered.");
                }
            }

            return ServiceSlot<T>.Instance;
        }

        /// <summary>
        /// Resolves and retrieves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="T">The type of the service to resolve.</typeparam>
        /// <returns>Returns the resolved instance of the specified service type.</returns>
        public static void Resolve<T>(Action<T> serviceRequest)
        {
            if (serviceRequest == null)
            {
                Debug.LogWarning($"Action<T> callback for resolving {typeof(T)} is null!!!");
                return;
            }

            if (ServiceSlot<T>.HasInstance)
            {
                serviceRequest(ServiceSlot<T>.Instance);
            }
            else
            {
                ServiceSlot<T>.EnsureRegisteredForReset();
                ServiceSlot<T>.AddRequest(serviceRequest);
            }
        }

        /// <summary>
        /// Unregisters a previously registered service of the specified type, removing its instance and making it unavailable for retrieval.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        /// <returns>Returns the unregistered service instance (for calling dispose and cleanup in the service), or the default value of the type if no instance was registered.</returns>
        public static T Unregister<T>()
        {
            var inst = ServiceSlot<T>.Instance;
            ServiceSlot<T>.ClearInstance();
            return inst;
        }

        /// <summary>
        /// Unregisters a previously registered lazy-loaded service of the specified type,
        /// removing its factory and instance, and making it unavailable for retrieval.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        /// <returns>Returns the unregistered service instance, or the default value of the type if no instance was registered.</returns>
        public static T UnregisterLazy<T>()
        {
            var inst = Unregister<T>();
            ServiceSlot<T>.Factory = null;
            return inst;
        }

        /// <summary>
        /// Removes a service request from the service locator for the specified type and action.
        /// </summary>
        /// <typeparam name="T">The type of the service for which the request is to be removed.</typeparam>
        /// <param name="serviceRequest">The action representing the service request to remove.</param>
        public static void RemoveServiceRequest<T>(Action<T> serviceRequest)
        {
            if (serviceRequest == null) return;
            ServiceSlot<T>.RemoveRequest(serviceRequest);
        }

        /// <summary>
        /// Determines whether a service of the specified type is currently registered.
        /// </summary>
        /// <typeparam name="T">The type of the service to check for registration.</typeparam>
        /// <returns>Returns true if the service is registered; otherwise, false.</returns>
        public static bool IsRegistered<T>() => ServiceSlot<T>.HasInstance;

        /// <summary>
        /// Unregisters all services! Use with caution! The intent is for use with testing and debugging. Resets the state of all registered services and clears their associated reset actions.
        /// </summary>
        /// <remarks>
        /// This method clears all registered reset actions, ensuring any per-service state is reset.
        /// It invokes each registered reset action in the order they were added, handling any exceptions that may occur during their execution.
        /// After invocation, the reset actions are removed from the list.
        /// </remarks>
        public static void Reset()
        {
            List<Action> snapshot;
            lock (ResettersLock)
            {
                // Copy to avoid issues if resetters modify the list
                snapshot = new List<Action>(Resetters);
                Resetters.Clear();
            }

            // Invoke each per-T resetter
            foreach (var reset in snapshot)
            {
                try
                {
                    reset?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        // ——— Internal typed storage per T ———

        /*
         * Keeps one static "slot" per closed generic type T.
         * Because these fields are static inside a private static class ServiceSlot<T>,
         * each distinct T gets its own separate storage (no dictionary lookups) and gets its own distinct set of static members:
         * ServiceSlot<Foo> is a different type with different static fields than ServiceSlot<Bar>.
         */
        private static class ServiceSlot<T>
        {
            internal static T Instance;                // the stored service value/reference
            internal static bool HasInstance;          // guard flag for whether Instance is set
            internal static Func<T> Factory;           // lazy factory delegate
            internal static readonly object SlotLock = new object(); // per‑T lock

            private static event Action<T> Requests;   // queued callbacks awaiting the instance
            internal static bool HasPendingRequests => Requests != null;

            private static bool _registeredForReset;   // ensures we add reset hook once

            internal static void EnsureRegisteredForReset()
            {
                //This is a double‑checked pattern. The outer check is a fast path, the inner check guarantees only one thread wins when there’s contention.
                if (_registeredForReset) return;
                /*
                 * We lock here to make the operations on the shared registry (Resetters) and the per‑type guard flag (_registeredForReset) atomic and thread‑safe.
                 * Without the lock, two threads could both think they’re the “first” for the same T and add duplicate resetters,
                 * or Reset() could read/clear the list while another thread is adding to it — classic race conditions.
                 */
                lock (ResettersLock)
                {
                    if (_registeredForReset) return;
                    Resetters.Add(ResetSlot);
                    _registeredForReset = true;
                }
            }

            internal static void AddRequest(Action<T> req) => Requests += req;
            internal static void RemoveRequest(Action<T> req) => Requests -= req;

            internal static void Notify(T value)
            {
                // Notify and clear queued requests
                Requests?.Invoke(value);
                Requests = null;
            }

            internal static void ClearInstance()
            {
                Instance = default;
                HasInstance = false;
            }

            // Called by global Reset(); clears everything for this T and allows re-registration in the registry
            private static void ResetSlot()
            {
                Instance = default;
                HasInstance = false;
                Factory = null;
                Requests = null;
                _registeredForReset = false; // allow re-add on next touch
            }
        }
    }
}