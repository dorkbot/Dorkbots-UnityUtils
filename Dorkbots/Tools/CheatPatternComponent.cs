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
using Signals;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Dorkbots.MonoBehaviorUtils;

namespace Dorkbots.Tools
{
    public class CheatPatternComponent : MonoBehaviour 
    {
        [SerializeField] private float timeBeforeDestroy = 0;
        [SerializeField] private float timeBetweenClicks = 2;
        [SerializeField] private Button[] buttons;

        public Signal cheatActivatedSignal{ get; private set; }

        private bool[] buttonStates;
        private Coroutine cheatPatternCoroutine;
        private Coroutine timerCoroutine;

        // Use this for initialization
        void Awake () 
        {
            cheatActivatedSignal = new Signal();
            buttonStates = new bool[buttons.Length];
        }

        void Start()
        {
            Button button;
            if (buttons.Length == 1)
            {
                buttonStates[0] = false;
                buttons[0].onClick.AddListener(() => OnlyButtonClicked());
            }
            else if (buttons.Length > 1)
            {
                int lastButtonPosition = buttons.Length - 1;
                for (int i = 0; i < buttons.Length; i++)
                {
                    button = buttons[i];
                    buttonStates[i] = false;
                    if (i == 0)
                    {
                        button.onClick.AddListener(() => FirstButtonClicked());   
                    }
                    else if (i == lastButtonPosition)
                    {
                        button.onClick.AddListener(() => LastButtonClicked());
                    }
                    else
                    {
                        button.onClick.AddListener(() => MiddleButtonClicked());
                    }
                }
            }

            if (timeBeforeDestroy > 0) StartStopCoroutine.StartCoroutine(ref timerCoroutine, TimerEnumerator(), this);
        }

        void OnDestroy()
        {
            if (cheatActivatedSignal != null)
            {
                cheatActivatedSignal.Dispose();
                cheatActivatedSignal = null;
            }

            StartStopCoroutine.StopCoroutine(ref timerCoroutine, this);
            StartStopCoroutine.StopCoroutine(ref cheatPatternCoroutine, this);
        }

        private IEnumerator TimerEnumerator()
        {
            yield return new WaitForSeconds(timeBeforeDestroy);

            Destroy(this);
        }

        private IEnumerator CheatPatternCoroutine()
        {
            yield return new WaitForSeconds(timeBetweenClicks);
            ResetCheatButton();
        }

        private bool CheckCheatButtons(int startPosition)
        {
            for (int i = startPosition; i >= 0; i--)
            {
                if (buttonStates[i] == false)
                {
                    return false;
                }
            }

            return true;
        }

        private void ResetCheatButton()
        {
            for (int i = 0; i < buttonStates.Length; i++)
            {
                buttonStates[i] = false;
            }
        }

        private bool SetCheatButton(int position)
        {
            if (CheckCheatButtons(position - 1))
            {
                buttonStates[position] = true;
                StartStopCoroutine.StartCoroutine(ref cheatPatternCoroutine, CheatPatternCoroutine(), this);

                return true;
            }
            else
            {
                ResetCheatButton();

                return false;
            }
        }

        private void ActivateCheat()
        {
            cheatActivatedSignal.Dispatch();

            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }

            Destroy(this);
        }

        // BUTTON HANDLERS
        private void FirstButtonClicked()
        {
            if (!CheckCheatButtons(buttonStates.Length - 1))
            {
                buttonStates[0] = true;
                StartStopCoroutine.StartCoroutine(ref cheatPatternCoroutine, CheatPatternCoroutine(), this);
            }
            else
            {
                ResetCheatButton();
            }   
        }

        private void MiddleButtonClicked()
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            SetCheatButton(Array.IndexOf(buttons, button));
        }

        private void LastButtonClicked()
        {
            if (SetCheatButton(buttons.Length - 1))
            {
                ActivateCheat();
            }
        }

        private void OnlyButtonClicked()
        {
            ActivateCheat();
        }
    }
}