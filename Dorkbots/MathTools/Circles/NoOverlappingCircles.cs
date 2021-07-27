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
using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.MathTools.Circles
{
	public class NoOverlappingCircles
	{
		public NoOverlappingCircles()
		{
			
		}

        public static void Place2DCircles(ICircle[] newCircles, ICircle[] oldCircles, float xMin, float xMax, float yMin, float yMax, float buffer = 0, bool place = true)
		{
            int currentArrayPosition = oldCircles.Length;
			uint attempts = 0;
			int i = 0;
			bool continueLoop = true;
			bool continueLookingForPosition = true;
			bool foundPosition = false;
			Vector3 newPosition;
			ICircle currentCircle;
			ICircle testingCircle;
            List<ICircle> tempList = new List<ICircle>();
            tempList.AddRange(oldCircles);
            tempList.AddRange(newCircles);

            ICircle[] circles = tempList.ToArray();

            // set new position to current position so we can use the newCirclePosition for avoiding overlap
            for (int j = 0; j < circles.Length; j++)
            {
                circles[j].newCirclePosition = circles[i].gameObject.transform.localPosition;
            }

			// Top loop
			while(continueLoop)
			{
				attempts = 0;

				continueLookingForPosition = true;

				// place a circle
				while(continueLookingForPosition)
				{
					currentCircle = circles[currentArrayPosition];

                    newPosition = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), currentCircle.gameObject.transform.position.z);
					foundPosition = true;

					i = 0;

					// test new position
					while(foundPosition && i < circles.Length)
					{
						testingCircle = circles [i];
                        if (currentCircle != testingCircle && Vector3.Distance(testingCircle.newCirclePosition, newPosition) < (currentCircle.GetRadius() + testingCircle.GetRadius() + buffer))
						{
							// position too close
							foundPosition = false;
						}
						i++;
					}

					if (foundPosition)
					{
                        currentCircle.newCirclePosition = newPosition;
						if (place) currentCircle.gameObject.transform.localPosition = newPosition;
						continueLookingForPosition = false;
					}
					else
					{
						// position was not found, try again
						attempts++;
						if (attempts >= 5000)
						{
                            //Debug.Log("ax attempts made!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            // max attempts made
							continueLookingForPosition = false;
							continueLoop = false;
							break;
						}
					}
				}

				currentArrayPosition++; 

				// all circles have been placed.
				if (currentArrayPosition >= circles.Length)
				{
					continueLoop = false;
					break;
				}
			}
        }
	}
}