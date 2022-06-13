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
    /// This class is used in the Service Locator Pattern (read more -> https://en.wikipedia.org/wiki/Service_locator_pattern).
    /// The service locator pattern is a design pattern used in software development to encapsulate the processes involved in obtaining a service with a strong abstraction layer.
    /// This pattern uses a central registry known as the “service locator” which on request returns the information necessary to perform a certain task.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        /// <summary>
        /// You need to register the service before it can be Resolved or accessed. You can only register one instance of one type.
        /// </summary>
        /// <param name="serviceInstance">An instance of the service</param>
        /// <typeparam name="T">The service type</typeparam>
        public static T Register<T>(object serviceInstance)
        {
            if (Services.ContainsKey(typeof(T)))
            {
                Debug.LogError("Service is already registered! Please unregister the current service before registering a new instance.");
            }
            else if (serviceInstance is T)
            {
                Services[typeof(T)] = serviceInstance;
            }
            else
            {
                Debug.LogError("Object does not derived from the service type!");
            }

            return (T)serviceInstance;
        }

        /// <summary>
        /// Unregisters current instance of a service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        public static void Unregister<T>()
        {
            if (Services.ContainsKey(typeof(T)))
            {
                Services.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Returns the service. This will fail if the service was not first registered via the `Register` method.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>Returns a registered instance of the type requested.</returns>
        public static T Resolve<T>()
        {
            return (T)Services[typeof(T)];
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