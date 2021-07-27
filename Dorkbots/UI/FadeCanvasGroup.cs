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
using System.Collections;
using Dorkbots.MonoBehaviorUtils;
using Signals;
using UnityEngine;

namespace Dorkbots.UI
{
    public class FadeCanvasGroup
    {
        public Signal fadeDownComplete { get; private set; }
        public Signal fadeUpComplete { get; private set; }

        private CanvasGroup canvasGroupFadingDown;
        private CanvasGroup canvasGroupFadingUp;
        private Coroutine fadeUpCoroutine;
        private Coroutine fadeDownCoroutine;
        private MonoBehaviour monoBehaviour;
        private float waitSeconds = .03f;
        private float fadeDownUpSeconds;

        public FadeCanvasGroup(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
            fadeDownComplete = new Signal();
            fadeUpComplete = new Signal();
        }

        public void FadeDownUP(CanvasGroup fromCanvasGroup, CanvasGroup toCanvasGroup, float seconds, float startDelay = 0)
        {
            canvasGroupFadingUp = toCanvasGroup;
            fadeDownUpSeconds = seconds;
            fadeDownComplete.Add(FadeDownCompleteHandler);
            FadeDown(fromCanvasGroup, seconds, startDelay);
        }

        private void FadeDownCompleteHandler()
        {
            fadeDownComplete.Remove(FadeDownCompleteHandler);
            FadeUp(canvasGroupFadingUp, fadeDownUpSeconds);
        }

        public void CrossFade(CanvasGroup fromCanvasGroup, CanvasGroup toCanvasGroup, float seconds, float startDelay = 0)
        {
            // cross fade
        }

        public void FadeUp(CanvasGroup canvasGroup, float seconds, float startDelay = 0)
        {
            canvasGroupFadingUp = canvasGroup;
            canvasGroupFadingUp.gameObject.SetActive(true);
            //if (canvasGroupFadingUp.alpha >= 1) 
            canvasGroupFadingUp.alpha = 0;
            float fadeStep = GetFadeStep(seconds);
            StartStopCoroutine.StartCoroutine(ref fadeUpCoroutine, FadeUpEnumerator(fadeStep, startDelay), monoBehaviour);
        }

        public void FadeDown(CanvasGroup canvasGroup, float seconds, float startDelay = 0)
        {
            canvasGroupFadingDown = canvasGroup;
            canvasGroupFadingDown.gameObject.SetActive(true);
            //if (canvasGroupFadingDown.alpha <= 0) 
            canvasGroupFadingDown.alpha = 1;
            float fadeStep = GetFadeStep(seconds);
            StartStopCoroutine.StartCoroutine(ref fadeDownCoroutine, FadeDownEnumerator(fadeStep, startDelay), monoBehaviour);
        }

        public void Stop()
        {
            StartStopCoroutine.StopCoroutine(ref fadeUpCoroutine, monoBehaviour);
            StartStopCoroutine.StopCoroutine(ref fadeDownCoroutine, monoBehaviour);
        }

        public void Reset()
        {
            Stop();
            if (canvasGroupFadingUp != null) canvasGroupFadingUp.alpha = 1;
            if (canvasGroupFadingDown != null) canvasGroupFadingDown.alpha = 1;
        }

        public void ResetCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
        }

        public void Dispose()
        {
            Stop();
            fadeDownComplete.Dispose();
            fadeDownComplete = null;
            fadeUpComplete.Dispose();
            fadeUpComplete = null;
        }

        private IEnumerator FadeUpEnumerator(float fadeStep, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);

            while (canvasGroupFadingUp.alpha < 1)
            {
                canvasGroupFadingUp.alpha += fadeStep;

                // roughly 30 times a second
                yield return new WaitForSeconds(waitSeconds);
            }

            fadeUpComplete.Dispatch();
        }

        private IEnumerator FadeDownEnumerator(float fadeStep, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);

            while (canvasGroupFadingDown.alpha > 0)
            {
                canvasGroupFadingDown.alpha -= fadeStep;

                // roughly 30 times a second
                yield return new WaitForSeconds(waitSeconds);
            }

            canvasGroupFadingDown.gameObject.SetActive(false);

            fadeDownComplete.Dispatch();
        }

        private float GetFadeStep(float seconds)
        {
            return waitSeconds / seconds;
        }
    }
}
