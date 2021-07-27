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
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Dorkbots.VR.Vive
{
    [RequireComponent(typeof(Interactable))]
    public class RotateDrive : MonoBehaviour
    {
        [Tooltip("Child GameObject which has the Collider component to initiate interaction, only needs to be set if there is more than one Collider child")]
        public Collider childCollider = null;

        [Tooltip("If true, the drive will stay manipulating as long as the button is held down, if false, it will stop if the controller moves out of the collider")]
        public bool hoverLock = false;

        private bool driving = false;

        private Hand handHoverLocked = null;

        private Interactable interactable;

        private GrabTypes grabbedWithType;
        private Quaternion delta;

        private void Awake()
        {
            interactable = this.GetComponent<Interactable>();
        }

       void OnDisable()
        {
            if (handHoverLocked)
            {
                handHoverLocked.HideGrabHint();
                handHoverLocked.HoverUnlock(interactable);
                handHoverLocked = null;
            }
        }

        private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount)
        {
            if (hand != null)
            {
                int nRangeMax = (int)Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
                nCount = Mathf.Clamp(nCount, 1, 10);

                float hapticDuration = nRangeMax * nCount;

                hand.TriggerHapticPulse(hapticDuration, nRangeMax, flMagnitude);

                for (ushort i = 0; i < nCount; ++i)
                {
                    ushort duration = (ushort)Random.Range(100, nRangeMax);
                    hand.TriggerHapticPulse(duration);
                    yield return new WaitForSeconds(.01f);
                }
            }
        }

        
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            hand.ShowGrabHint();
        }

        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            hand.HideGrabHint();

            if (driving && hand)
            {
                //hand.TriggerHapticPulse() //todo: fix
                StartCoroutine(HapticPulses(hand, 1.0f, 10));
            }

            driving = false;
            handHoverLocked = null;
        }

        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
            {
                grabbedWithType = startingGrabType;
                if (hoverLock)
                {
                    hand.HoverLock(interactable);
                    handHoverLocked = hand;
                }

                hand.HideGrabHint();

                driving = true;

                var rot = Quaternion.LookRotation(hand.hoverSphereTransform.position - this.transform.position);
                //this is how you calculate the amount of rotation from a->b. It's inv(a) * b.
                //note I'm going from lookAt to rotation, because we want to 'remove' it.
                delta = Quaternion.Inverse(rot) * this.transform.rotation;
            }
            else if (grabbedWithType != GrabTypes.None && isGrabEnding)
            {
                // Trigger was just released
                if (hoverLock)
                {
                    hand.HoverUnlock(interactable);
                    handHoverLocked = null;
                }

                driving = false;
                grabbedWithType = GrabTypes.None;
            }

            if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable)
            {
               this.transform.rotation = Quaternion.LookRotation(hand.hoverSphereTransform.position - this.transform.position) * delta;
            }
        }
    }
}
