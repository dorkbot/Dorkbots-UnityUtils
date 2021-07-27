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
using Valve.VR.InteractionSystem;

namespace Dorkbots.VR.Vive
{
    public class LinearRange : MonoBehaviour
    {
        [SerializeField] private LinearMapping linearMappingMax;
        [SerializeField] private LinearRangeDrive linearDriveMax;
        [SerializeField] private LinearMapping linearMappingMin;
        [SerializeField] private LinearRangeDrive linearDriveMin;

        private float maxValue;
        private float minValue;

        private void Start()
        {
            maxValue = linearMappingMax.value;
            minValue = linearMappingMin.value;

            linearDriveMax.min = minValue;
            linearDriveMin.max = maxValue;
        }

        void Update()
        {
            if (maxValue != linearMappingMax.value)
            {
                maxValue = linearMappingMax.value;

                if (maxValue < linearMappingMin.value)
                {
                    maxValue = linearMappingMax.value = linearMappingMin.value;
                }

                linearDriveMin.max = maxValue;
            }

            if (minValue != linearMappingMin.value)
            {
                minValue = linearMappingMin.value;

                if (minValue > linearMappingMax.value)
                {
                    minValue = linearMappingMin.value = linearMappingMax.value;
                }

                linearDriveMax.min = minValue;
            }
        }
    }
}
