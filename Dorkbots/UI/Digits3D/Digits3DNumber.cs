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

using UnityEngine;

namespace Dorkbots.UI.Digits3D
{
    public class Digits3DNumber : MonoBehaviour
    {
        [SerializeField] private int number = 0;
        [SerializeField] private DigitSource digitSource;
        [SerializeField] private Digit[] digits;

        private void OnValidate()
        {
            for (int i = 0; i < digits.Length; i++)
            {
                foreach (Transform child in digits[i].transform)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(child.gameObject);
                    };
                }
            }

            SetNumber(number, false);
        }

        private void Start()
        {
            SetNumber(number);
        }

        public void SetNumber(int number, bool destroyChildren = true)
        {
            int workingNumber = number;
            int i = 0;

            while (i < digits.Length && workingNumber > 0)
            {
                int power = (int)Mathf.Pow(10, i);
                int mod = (int)Mathf.Pow(10, i + 1);
                int digit = (workingNumber % mod) / power;
                digit = Mathf.Clamp(digit, 0, 9);//make sure number is never double digits

                if (destroyChildren)
                {
                    foreach (Transform child in digits[i].containerTransform)
                    {
                        Destroy(child.gameObject);
                    }
                }

                AddDigitArt(digitSource.Get(digit), digits[i].containerTransform);              

                workingNumber -= digit * power;
                i++;
            }

            for (; i < digits.Length; i++)
            {
                AddDigitArt(digitSource.Get(0), digits[i].containerTransform);
            }
        }

        private void AddDigitArt(GameObject digitPrefab, Transform containerTransform)
        {
            GameObject digitGO = Instantiate(digitPrefab, containerTransform);
            digitGO.transform.localPosition = Vector3.zero;
            digitGO.transform.localRotation = new Quaternion();
            digitGO.transform.localScale = Vector3.one;
        }
    }
}