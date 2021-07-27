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
using UnityEngine.UI;

namespace Dorkbots.Points
{
    public static class PointTools
    {
        /// <summary>
        /// Using the a screen position gets the position in a UI RawImage.</summary>
        /// <param name="rawImage">The target RawImage.</param>
        /// <param name="screenPosition">The screen point.</param>
        /// <returns>A Point object with the int x and y for the position.</returns>
        public static Point GetPointInRawImageFromScreenPoint(RawImage rawImage, Vector2 screenPosition)
        {
            // Get X and Y position of the users input on the source.
            Rect sourceRect = rawImage.rectTransform.rect;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, screenPosition, Camera.main, out localPoint);
            int inputX = Mathf.Clamp (0, (int)( ( (localPoint.x - sourceRect.x) * rawImage.texture.width) / sourceRect.width), rawImage.texture.width);
            int inputY = Mathf.Clamp (0, (int)( ( (localPoint.y - sourceRect.y) * rawImage.texture.height) / sourceRect.height), rawImage.texture.height);
            //Debug.Log ("input x = " + inputX + " || input y = " + inputY);

            return new Point(inputX, inputY);
        }
            
        /// <summary>
        /// Takes two points and creates a Rect.</summary>
        /// <param name="pointA">One of two Point objects.</param>
        /// <param name="pointB">Two of two Point objects.</param>
        /// <param name="positionBuffer">Adds a buffer to each point, use this to increase the height and width of the Rect.</param>
        /// <returns>Returns the Rect object created by the two points.</returns>
        public static Rect GetRectFromTwoPoints(Point pointA, Point pointB, int positionBuffer = 0)
        {
            // between points find x furthest to the left, and y furthest down. start point.
            // between points find the x furthest to the right, and y furthest up. end point.
            // x = 0, y = 0 is bottom left
            Point startPoint = new Point();
            Point endPoint = new Point();
            if (pointA.x < pointB.x)
            {
                startPoint.x = pointA.x;
                endPoint.x = (int)pointB.x;
            }
            else
            {
                startPoint.x = (int)pointB.x;
                endPoint.x = pointA.x;
            }

            if (pointA.y < pointB.y)
            {
                startPoint.y = pointA.y;
                endPoint.y = (int)pointB.y;
            }
            else
            {
                startPoint.y = (int)pointB.y;
                endPoint.y = pointA.y;
            }

            // add buffer to start point, move point to the left and down.
            startPoint.x = Mathf.Max(0, startPoint.x - positionBuffer);
            startPoint.y = Mathf.Max(0, startPoint.y - positionBuffer);
            // add buffer to end point, move point to the right and up. Heatmap deals with x and y being out of bounds.
            endPoint.x += positionBuffer;
            endPoint.y += positionBuffer;

            // between points find difference, this is the width and height.
            int width = endPoint.x - startPoint.x;
            int height = endPoint.y - startPoint.y;

           //Debug.Log("startPoint.x = " + startPoint.x + " || startPoint.y = " + startPoint.y + " || endPoint.x = " + endPoint.x + " || endPoint.y = " + endPoint.y + " || width = " + width + " || height = " + height);

            return new Rect(startPoint.x, startPoint.y, width, height);
        }
    }
}