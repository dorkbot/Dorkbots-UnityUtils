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
using UnityEngine;
using System.Collections;
using System;

namespace Dorkbots.MonoBehaviorTools
{
    public class StartStopCoroutine
    {
        /// <summary>
        /// Starts a Coroutine.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="enumerator"></param>
        /// <param name="parent"></param>
        public static void StartCoroutine(ref Coroutine reference, IEnumerator enumerator, MonoBehaviour parent)
        {
            if (reference != null)
            {
                parent.StopCoroutine(reference);
            }

            reference = parent.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stops Coroutine
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="parent"></param>
        public static void StopCoroutine(ref Coroutine reference, MonoBehaviour parent)
        {
            if (reference != null)
            {
                parent.StopCoroutine(reference);
                reference = null;
            }
        }

        /// <summary>
        /// Creates and returns an object that will invoke a method using a Coroutine. It auto starts. You can use the return object to stop the Coroutine.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="parent"></param>
        /// /// <param name="callback"></param>
        /// <returns>You can use the return object to stop the Coroutine.</returns>
        public static SimpleTimerCoroutine CreateSimpleTimerCoroutine(float time, MonoBehaviour parent, Action callback)//TODO: haven't tested
        {
            SimpleTimerCoroutine simpleTimerCoroutine = new SimpleTimerCoroutine(time, parent, callback);
            return simpleTimerCoroutine;
        }
        
        /// <summary>
        /// Creates and returns an object that will invoke a method as it lerps from one float to another within a set time. It auto starts. You can use the return object to stop the Coroutine.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        /// <returns>You can use the return object to stop the Coroutine.</returns>
        public static TimeLerpCoroutine CreateTimeLerpCoroutine(float duration, float startNum, float endNum, MonoBehaviour parent, Action<float> callback)//TODO: haven't tested
        {
            TimeLerpCoroutine timeLerpCoroutine = new TimeLerpCoroutine(duration, startNum, endNum, parent, callback);
            return timeLerpCoroutine;
        }
    }

    public class SimpleTimerCoroutine
    {
        private Coroutine _coroutine;
        private float _time;
        private Action _callback;
        private MonoBehaviour _parent;

        public SimpleTimerCoroutine(float time, MonoBehaviour parent, Action callback)
        {            
            _time = time;
            _callback = callback;
            _parent = parent;
        }

        /// <summary>
        /// Start Coroutine
        /// </summary>
        public void Start()
        {
            StartStopCoroutine.StartCoroutine(ref _coroutine, Enumerator(), _parent);
        }

        /// <summary>
        /// Stop the Coroutine.
        /// </summary>
        public void Stop()
        {
            StartStopCoroutine.StopCoroutine(ref _coroutine, _parent);
        }

        /// <summary>
        /// Dispose and null references
        /// </summary>
        public void Dispose()
        {
            Stop();
            _callback = null;
            _parent = null;
        }

        private IEnumerator Enumerator()
        {
            yield return new WaitForSeconds(_time);

            _callback();
        }
    }

    public class TimeLerpCoroutine
    {
        private Coroutine _coroutine;
        private float _duration;
        private float _startNum;
        private float _endNum;
        private Action<float> _callback;
        private MonoBehaviour _parent;

        /// <summary>
        /// Iinvokes a method as it lerps from one float to another within a set time.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public TimeLerpCoroutine(float duration, float startNum, float endNum, MonoBehaviour parent, Action<float> callback)
        {            
            _duration = duration;
            _startNum = startNum;
            _endNum = endNum;
            _callback = callback;
            _parent = parent;
        }

        /// <summary>
        /// Start Coroutine
        /// </summary>
        public void Start()
        {
            StartStopCoroutine.StartCoroutine(ref _coroutine, Enumerator(), _parent);
        }

        /// <summary>
        /// Stop the Coroutine.
        /// </summary>
        public void Stop()
        {
            StartStopCoroutine.StopCoroutine(ref _coroutine, _parent);
        }

        /// <summary>
        /// Dispose and null references
        /// </summary>
        public void Dispose()
        {
            Stop();
            _callback = null;
            _parent = null;
        }

        private IEnumerator Enumerator()
        {
            float t = 0;
            while(t < 1)
            {
                _callback(Mathf.Lerp(_startNum ,_endNum, t));
                t = t + Time.deltaTime / _duration;
                yield return new WaitForEndOfFrame();
            }

            _callback(_endNum);
        }
    }
}