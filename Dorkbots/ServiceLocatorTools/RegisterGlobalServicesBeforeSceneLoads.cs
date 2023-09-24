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

/*
 * This is a template. Copy it and put it in the project's script folder. Then make changes (ie: change name space, uncomment out code and add code for Registering project services, etc.)
 * This will Register services before a scene loads so other classes can access the services with no issues.
 */
using Dorkbots.ServiceLocatorTools;
using UnityEngine;

namespace Dorkbots.ServiceLocatorTools
{
    /* Uncomment this out to use...
    public class RegisterGlobalServices
    {
        public void RegisterServices()
        {
           //Register services here
        }

        public void UnregisterServices()
        {
            //Unregister services here
        }
    }

    ///
    /// Update above for the project. Below handles the RuntimeInitializeOnLoadMethod event
    ///

    public class RegisterGlobalServicesBeforeSceneLoads
    {
        private static RegisterGlobalServices _registerGlobalServices = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//this seems to work but keep an eye on it...
        public static void RegisterServices()
        {
            if (_registerGlobalServices == null)
            {
                _registerGlobalServices = new RegisterGlobalServices();
                _registerGlobalServices.RegisterServices();
            }
        }

        public static void UnregisterServices()
        {
            if (_registerGlobalServices != null)
            {
                _registerGlobalServices.UnregisterServices();
                _registerGlobalServices = null;
            }
        }
    }*/
}