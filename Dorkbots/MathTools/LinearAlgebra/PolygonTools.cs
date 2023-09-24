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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.MathTools.LinearAlgebra
{
    public class PolygonTools
    {
        /// <summary>
        /// Returns the closest point on a polygon's edge out of a list of polygons
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="polygons">Polygons to search</param>
        /// <param name="closestDistance">The distance of the closest point</param>
        /// <param name="closestPolygon">The Closest Polygon</param>
        /// <returns>Returns the closest point on the edge of a polygon</returns>
        public static Vector3 ClosestPointOnEdge(Vector3 position, List<Vector3[]> polygons, out float closestDistance, out Vector3[] closestPolygon)
        {
            Vector3[] currentPolygon;
            Vector3 currentPointOnPolygonEdge;
            float currentDistance;

            closestPolygon = Array.Empty<Vector3>();
            closestDistance = float.PositiveInfinity;
            Vector3 closestPointOnPolygonEdge = Vector3.positiveInfinity;
            
            for (int i = 0; i < polygons.Count; i++)
            {
                currentPolygon = polygons[i];
                currentPointOnPolygonEdge = ClosestPointOnEdge(position, currentPolygon);
                currentDistance = Vector3.Distance(currentPointOnPolygonEdge, position);
                if (currentDistance < closestDistance)
                {
                    closestPolygon = currentPolygon;
                    closestDistance = currentDistance;
                    closestPointOnPolygonEdge = currentPointOnPolygonEdge;
                }
            }

            return closestPointOnPolygonEdge;
        }

        /// <summary>
        /// Returns the closet point that lies on the edge of the polygon
        /// </summary>
        /// <param name="polygon">An array of Vector3 points that make up the polygon</param>
        /// <param name="position">The position we are using</param>
        /// <returns>The position on the surface of the polygon</returns>
        /// <exception cref="ArgumentException">Throws an exception if polygon only has one point</exception>
        public static Vector3 ClosestPointOnEdge(Vector3 position, Vector3[] polygon)
        {
            if (polygon.Length < 3) throw new ArgumentException("Polygon must have 3 points!");
            
            Vector3 lineStartPos;
            Vector3 lineEndPos;
            Vector3 nearestPoint = new Vector3();// the nearest point found so far
            Vector3 pointOnLine;// point on line that is currently being tested
            float nearestPointDistance = float.PositiveInfinity;// the distance of the current nearest point
            float distanceOfCurrentTestingPoint;// the distance of the currently being tested point
            
            int lastArrayPosition = polygon.Length - 1;

            for (int i = 0; i < polygon.Length; i++)
            {
                if (i == lastArrayPosition)// last point and line
                {
                    lineStartPos = polygon[i];
                    lineEndPos = polygon[0];
                }
                else
                {
                    lineStartPos = polygon[i];
                    lineEndPos = polygon[i + 1];
                }

                pointOnLine = LineTools.NearestPointOnLine(lineStartPos, lineEndPos, position);
                distanceOfCurrentTestingPoint = Vector3.Distance(position, pointOnLine);
                if (i == 0)//first test
                {
                    nearestPoint = pointOnLine;
                    nearestPointDistance = distanceOfCurrentTestingPoint;
                }
                else if (distanceOfCurrentTestingPoint < nearestPointDistance)
                {
                    nearestPoint = pointOnLine;
                    nearestPointDistance = distanceOfCurrentTestingPoint;
                }
            }
            
            return nearestPoint;
        }

        /// <summary>
        /// Finds the center of centroid of a polygon
        /// </summary>
        /// <param name="polygon">A array of points that make up the polygon</param>
        /// <returns></returns>
        public static Vector3 Centroid(Vector3[] polygon)
        {
            Vector3 positionSum = Vector3.zero;
            for (int i = 0; i < polygon.Length; i++)
            {
                positionSum += polygon[i];
            }

            return positionSum / polygon.Length;
        }
        
        /// <summary>
        /// Resolves if a 2d point is inside a 2d polygon
        /// </summary>
        /// <param name="point">3D Position to test converted to 2D (y=z)</param>
        /// <param name="polygon">3D Polygon converted to 2D (y=z)</param>
        /// <returns>True if inside</returns>
        public static bool IsPointIn2DPolygon(Vector3 point, Vector3[] polygon)
        {
            Vector2[] polygon2D = new Vector2[polygon.Length];
            for (int i = 0; i < polygon.Length; i++)
            {
                polygon2D[i] = new Vector2(polygon[i].x, polygon[i].z);
            }

            Vector2 point2D = new Vector2(point.x, point.z);
            
            return IsPointIn2DPolygon(point2D, polygon2D);
        }
        
        /// <summary>
        /// Resolves if a 2d point is inside a 2d polygon
        /// </summary>
        /// <param name="point">Position to test</param>
        /// <param name="polygon">Polygon</param>
        /// <returns>True if inside</returns>
        public static bool IsPointIn2DPolygon(Vector2 point, Vector2[] polygon)
        {
            int polygonLength = polygon.Length;
            bool inside = false;
            
            // x, y for tested point.
            float pointX = point.x, pointY = point.y;
            
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];           
            endX = endPoint.x; 
            endY = endPoint.y;
            
            int i = 0;
            while (i < polygonLength) 
            {
                startX = endX;           
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;       
                endY = endPoint.y;
                //
                inside ^= ( endY > pointY ^ startY > pointY ) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
            }
            return inside;
        }
    }
}