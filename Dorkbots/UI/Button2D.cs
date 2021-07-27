/*
* Author: Dayvid jones
* http://www.dayvid.com
* Copyright (c) Superhero Robot 2018
* http://www.superherorobot.com
* Managed by Dorkbots
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
using Signals;
using UnityEngine;

namespace Dorkbots.UI
{
	[RequireComponent(typeof(Collider2D))]
	public class Button2D : MonoBehaviour 
	{
        public Signal<Button2D> mouseUpSignal { get; private set; }
        public Signal<Button2D> mouseDownSignal { get; private set; }

        private bool perform = true;

        private void Awake()
        {
            mouseUpSignal = new Signal<Button2D>();
            mouseDownSignal = new Signal<Button2D>();
        }

        void OnMouseDown() 
		{
            if (perform) mouseDownSignal.Dispatch(this);
		}

        private void OnMouseUp()
        {
            if (perform) mouseUpSignal.Dispatch(this);
        }

        void OnEnable()
        {
            perform = true;
        }

        void OnDisable()
        {
            perform = false;
        }
    }
}