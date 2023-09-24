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

namespace Dorkbots.MathTools.LinearAlgebra
{
    public class LineTools
    {
        /// <summary>
        /// Returns the nearest point (Vector3) on a line
        /// </summary>
        /// <param name="start">Start position of the line</param>
        /// <param name="end">End position of the line</param>
        /// <param name="point">The point</param>
        /// <returns>Returns the nearest position (Vector3) on the line</returns>
        public static Vector3 NearestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
        {
            // define the line
            Vector3 line = (end - start);
            float lineLength = line.magnitude;
            line.Normalize();
            // get the dot product
            Vector3 leftHandSide = point - start;
            float dot = Vector3.Dot(leftHandSide, line);
            dot = Mathf.Clamp(dot, 0f, lineLength);
            
            return start + line * dot;
        }
    }
}