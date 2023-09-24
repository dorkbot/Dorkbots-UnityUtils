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
using UnityEngine;

namespace Dorkbots.ServiceLocatorTools
{
    /// <summary>
    /// Because settings in Unity's Scripting Execution Order doesn't have expected results in different environments (ex: editor vs Android) use this to manage initializing the registering of scene specific services. Great for references to MonoBehaviors that are services.
    /// Extend this method, attach the concrete child script to a GameObject in a scene where services need to be register. Crease fields and attach MonoBehavior services, then Register them in the `PerformRegisterServices' abstract method.
    /// Then have a main encapsulating class (ie: Controller or Manager class) call the RegisterServices method.
    /// This solution is great for using MonoBehaviors as services!
    /// RegisteringServices is also called during the Awake method...
    /// </summary>
    public abstract class RegisterServicesManager : MonoBehaviour
    {
        public bool Registered { get; private set; }

        private event Action _registeredAction;

        protected void Awake()
        {
            RegisterServices();
        }

        /// <summary>
        /// Returns the bool value of Registered which represents the state of the services
        /// </summary>
        /// <param name="registeredCallback">If `Registered` is false then this callback is used when `Registered` is true.</param>
        /// <returns>Returns the value of `Registered`</returns>
        public bool CheckIfRegisteredAddCallback(Action registeredCallback)
        {
            if (!Registered) _registeredAction += registeredCallback;
            return Registered;
        }

        /// <summary>
        /// Call this to ensure that services are registered before attempting to resolve (access) them.
        /// </summary>
        public void RegisterServices()
        {
            if (!Registered)
            {
                PerformRegisterServices();
                _registeredAction?.Invoke();
                _registeredAction = null;
                Registered = true;
            }
        }

        protected abstract void PerformRegisterServices();

        protected void OnDestroy()
        {
            UnregisterServices();
        }

        /// <summary>
        /// Use this to unregister services. OnDestroy calls this as well...
        /// </summary>
        public void UnregisterServices()
        {
            if (Registered)
            {
                PerformUnregisterServices();
                Registered = false;
            }
        }

        protected abstract void PerformUnregisterServices();
    }
}