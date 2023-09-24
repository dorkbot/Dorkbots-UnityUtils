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
using System.Reflection;
using UnityEngine;

namespace Dorkbots.ServiceLocatorTools
{

    /// <summary>
    /// This class is used in the Service Locator Pattern (read more -> https://en.wikipedia.org/wiki/Service_locator_pattern).
    /// The service locator pattern is a design pattern used in software development to encapsulate the processes involved in obtaining a service with a strong abstraction layer.
    /// This pattern uses a central registry known as the “service locator” which on request returns the information necessary to perform a certain task.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, Type> LazyServices = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, Action<object>> ServiceRequests = new Dictionary<Type, Action<object>>();

        /// <summary>
        /// You need to register the service before it can be resolved or accessed. You can only register one instance of one type.
        /// </summary>
        /// <param name="serviceInstance">An instance of the service</param>
        /// <typeparam name="T">The service type</typeparam>
        public static T Register<T>(object serviceInstance)
        {
            return RegisterInstance<T>(serviceInstance, false);
        }

        /// <summary>
        /// Register a lazy service that is instantiated when resolved
        /// </summary>
        /// <param name="service">Service to instantiate</param>
        /// <typeparam name="T">The type of the service</typeparam>
        public static void RegisterLazy<T>(Type service)
        {
            if (LazyServices.ContainsKey(typeof(T)))
            {
                Debug.LogError("The Service " + typeof(T) + " is already registered as a Lazy Service! Please unregister the current lazy service before registering a new lazy service.");
            }
            else if (Services.ContainsKey(typeof(T)) || LazyServices.ContainsKey(typeof(T)))
            {
                Debug.LogError("The Service " + typeof(T) + " is already registered as a service! Please unregister the current service before registering a new lazy service.");
            }
            else if(typeof(T).IsAssignableFrom(service))
            {
                LazyServices[typeof(T)] = service;

                if (ServiceRequests.ContainsKey(typeof(T)))//someone has requested this service
                {
                    Resolve<T>();
                }
            }
            else
            {
                Debug.LogError("Service does not derive from the service type!");
            }
        }

        private static T RegisterInstance<T>(object serviceInstance, bool isLazy)
        {
            if (Services.ContainsKey(typeof(T)))
            {
                if (isLazy)
                {
                    Debug.LogError("An instance of the lazy service " + typeof(T) + " is already registered! Please unregister the current lazy service before registering a new instance.");
                }
                else
                {
                    Debug.LogError("The Service " + typeof(T) + " is already registered! Please unregister the current service before registering a new instance.");
                }
            }
            else if (!isLazy && LazyServices.ContainsKey(typeof(T)))
            {
                Debug.LogError("The service " + typeof(T) + " is already registered as a lazy service! Please unregister the current lazy service before registering a new instance.");
            }
            else if (serviceInstance is T)
            {
                Services[typeof(T)] = serviceInstance;

                if (ServiceRequests.ContainsKey(typeof(T)))
                {
                    ServiceRequests[typeof(T)]?.Invoke(serviceInstance);
                    ServiceRequests.Remove(typeof(T));
                }
            }
            else
            {
                Debug.LogError("Object does not derive from the service type!");
            }

            return (T)serviceInstance;
        }

        /// <summary>
        /// Unregisters current instance of a service. An warning is logged ff an instance is not found.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>A reference to the service being unregistered. Useful for calling a Dispose method. Returns default value or null if service instance is not found.</returns>
        public static T Unregister<T>()
        {
            T service = default;
            if (Services.ContainsKey(typeof(T)))
            {
                service = (T)Services[typeof(T)];
                Services.Remove(typeof(T));
            }
            else
            {
                Debug.LogWarning("The service " + typeof(T) + " doesn't exist!");
            }

            return service;
        }

        /// <summary>
        /// Unregisters lazy type to be instantiated. Also Unregisters the instance if one is Registered.
        /// </summary>
        /// <typeparam name="T">The type for the service</typeparam>
        /// <returns>Returns a registered instance if one exists</returns>
        public static T UnregisterLazy<T>()
        {
            T service = default;
            if (Services.ContainsKey(typeof(T)))
            {
                service = Unregister<T>();
            }

            if (LazyServices.ContainsKey(typeof(T)))
            {
                LazyServices.Remove(typeof(T));
            }
            else
            {
                Debug.LogWarning("The lazy service " + typeof(T) + " doesn't exist!");
            }

            return service;
        }

        /// <summary>
        /// Returns the service. This will fail if the service was not first registered via the `Register` method.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>Returns a registered instance of the type requested.</returns>
        public static T Resolve<T>()
        {
            if (Services.ContainsKey(typeof(T)))
            {
                return (T)Services[typeof(T)];
            }
            else if (LazyServices.ContainsKey(typeof(T)))
            {
                ConstructorInfo constructor = LazyServices[typeof(T)].GetConstructor(new Type[0]);
                Debug.Assert(constructor != null, "Cannot find a suitable constructor for " + typeof(T) + " lazy service.");

                return RegisterInstance<T>(constructor.Invoke(null), true);
            }
            else
            {
                Debug.LogError("The service " + typeof(T) + " has not been registered. Please register it before resolving.");
            }

            return default;
        }

        /// <summary>
        /// Use this method to Resolve for possible cases where Resolve could be called before the Type's object is registered.
        /// If the Type is not already registered, then the Action param will be Invoked at the time the Type is Registered.
        /// </summary>
        /// <param name="serviceRequest">The action to invoke when the object is registered. It's immediately invoked if the object is already registered.</param>
        /// <typeparam name="T">The service type</typeparam>
        public static void Resolve<T>(Action<object> serviceRequest)
        {
            if (Services.ContainsKey(typeof(T)))
            {
                serviceRequest?.Invoke(Services[typeof(T)]);
            }
            else
            {
                ServiceRequests[typeof(T)] += serviceRequest;
            }
        }

        /// <summary>
        /// Removes request
        /// </summary>
        /// <param name="serviceRequest">Action to be removed</param>
        /// <typeparam name="T">The service type</typeparam>
        public static void RemoveServiceRequest<T>(Action<object> serviceRequest)
        {
            if (ServiceRequests.ContainsKey(typeof(T)))
            {
                ServiceRequests[typeof(T)] -= serviceRequest;
                if (ServiceRequests[typeof(T)] == null)
                {
                    ServiceRequests.Remove(typeof(T));
                }
            }
        }

        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>returns true if the service is registered.</returns>
        public static bool IsRegistered<T>()
        {
            return Services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Clears all registered services. BUT does not perform any dispose or cleanup on the services.
        /// </summary>
        public static void Reset()
        {
            Services.Clear();
        }
    }
}