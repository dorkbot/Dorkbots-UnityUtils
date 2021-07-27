using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dorkbots.Tray
{
    public class TrayController
    {
        public enum Justified
        {
            Right,
            Left,
            None
        }

        public TrayController()
        {
            
        }

        public void CheckObjectSpacing(Tray tray)
        {
            Sweep(tray);
        }

        public void ObjectHover(TrayObjectDraggable trayObject, Tray tray)
        {
            if (tray.currentDimensionObjectCapacity >= tray.maxObjectCapacity)
            {
                NoSpaceEffect(tray);
                return;
            }

            bool movedObjects = false;
            tray.objects = new List<TrayObjectDraggable>(tray.objects.OrderBy(aObject => aObject.transform.position.x));

            float trayObjectX = trayObject.boxCollider.bounds.center.x;

            int i;
            for (i = 0; i < tray.objects.Count; i++)
            {
                if (tray.objects[i].boxCollider.bounds.center.x <= trayObjectX)
                {
                    tray.objectsToLeft.Add(tray.objects[i]);
                }
                else
                {
                    tray.objectsToRight.Add(tray.objects[i]);
                }
            }

            int j;
            float minXDiffOutOfTray;
            float maxXDiffOutOfTray;
            float xDiff;
            TrayObjectDraggable trayObjectMoved = trayObject;
            bool notTouching = false;
            // check if farthest left object is at edge or beyond of tray
            if (tray.objectsToLeft.Count > 0)
            {
                if (tray.boxCollider.bounds.min.x - tray.objectsToLeft[0].boxCollider.bounds.min.x == 0)
                {
                    // if true then see if all other blocks are touching
                    for (i = 0; i < tray.objectsToLeft.Count - 1; i++)
                    {
                        if (tray.objectsToLeft[i].boxCollider.bounds.max.x != tray.objectsToLeft[i + 1].boxCollider.bounds.min.x)
                        {
                            notTouching = true;
                        }
                    }
                }
                else
                {
                    notTouching = true;
                }
            }

            if (notTouching && tray.objectsToLeft.Count > 0)
            {
                tray.objectsToLeft.Reverse();

                //Debug.Log ("<Tray> BlockHover -> blocksToLeft.Count = " + blocksToLeft.Count);
                //Debug.Log ("<Tray> BlockHover -> blocksToRight.Count = " + blocksToRight.Count);

                // Left side
                for (i = 0; i < tray.objectsToLeft.Count; i++)
                {
                    if (trayObjectMoved.bounds.Intersects(tray.objectsToLeft[i].boxCollider.bounds))
                    {
                        xDiff = tray.objectsToLeft[i].boxCollider.bounds.max.x - trayObjectMoved.boxCollider.bounds.min.x;
                        //Debug.Log ("<Tray> BlockHover -> left xDiff = " + xDiff);
                        tray.objectsToLeft[i].transform.position = new Vector3(tray.objectsToLeft[i].transform.position.x - xDiff, tray.objectsToLeft[i].transform.position.y, tray.objectsToLeft[i].transform.position.z);
                        movedObjects = true;
                        trayObjectMoved = tray.objectsToLeft[i];
                        // check if too far to the left
                        minXDiffOutOfTray = tray.boxCollider.bounds.min.x - trayObjectMoved.boxCollider.bounds.min.x;
                        if (minXDiffOutOfTray > 0)
                        {
                            // move back
                            trayObjectMoved.transform.position = new Vector3(trayObjectMoved.transform.position.x + minXDiffOutOfTray, trayObjectMoved.transform.position.y, trayObjectMoved.transform.position.z);
                            tray.objectsToLeft = new List<TrayObjectDraggable>(tray.objectsToLeft.OrderBy(aBlock => aBlock.transform.position.x));
                            for (j = 0; j < tray.objectsToLeft.Count; j++)
                            {
                                //blocksToLeft [j].transform.position = new Vector3 (blocksToLeft [j].transform.position.x + minXDiffOutOfTray, blocksToLeft [j].transform.position.y, blocksToLeft [j].transform.position.z);
                                if (j + 1 <= tray.objectsToLeft.Count - 1 && tray.objectsToLeft[j].boxCollider.bounds.Intersects(tray.objectsToLeft[j + 1].boxCollider.bounds))
                                {
                                    xDiff = tray.objectsToLeft[j].boxCollider.bounds.max.x - tray.objectsToLeft[j + 1].boxCollider.bounds.min.x;
                                    tray.objectsToLeft[j + 1].transform.position = new Vector3(tray.objectsToLeft[j + 1].transform.position.x + xDiff, tray.objectsToLeft[j + 1].transform.position.y, tray.objectsToLeft[j + 1].transform.position.z);
                                }
                            }

                            break;
                        }
                    }
                }

            }

            tray.objectsToRight.Reverse();
            notTouching = false;
            // check if farthest rigth block is at edge or beyond of tray
            if (tray.objectsToRight.Count > 0)
            {
                if ((tray.boxCollider.bounds.max.x - tray.objectsToRight[0].boxCollider.bounds.max.x) == 0)
                {
                    // if true then see if all other blocks are touching
                    for (i = 0; i < tray.objectsToRight.Count - 1; i++)
                    {
                        if (tray.objectsToRight[i].boxCollider.bounds.min.x != tray.objectsToRight[i + 1].boxCollider.bounds.max.x)
                        {
                            notTouching = true;
                        }
                    }
                }
                else
                {
                    notTouching = true;
                }
            }

            if (notTouching && tray.objectsToRight.Count > 0)
            {
                tray.objectsToRight.Reverse();

                // Rigth side
                trayObjectMoved = trayObject;
                for (i = 0; i < tray.objectsToRight.Count; i++)
                {
                    if (trayObjectMoved.boxCollider.bounds.Intersects(tray.objectsToRight[i].boxCollider.bounds))
                    {
                        xDiff = trayObjectMoved.boxCollider.bounds.max.x - tray.objectsToRight[i].boxCollider.bounds.min.x;
                        //Debug.Log ("<Tray> BlockHover -> right xDiff = " + xDiff);
                        tray.objectsToRight[i].transform.position = new Vector3(tray.objectsToRight[i].transform.position.x + xDiff, tray.objectsToRight[i].transform.position.y, tray.objectsToRight[i].transform.position.z);
                        movedObjects = true;
                        trayObjectMoved = tray.objectsToRight[i];
                        // check if too far to the right
                        maxXDiffOutOfTray = tray.boxCollider.bounds.max.x - trayObjectMoved.boxCollider.bounds.max.x;
                        if (maxXDiffOutOfTray < 0)
                        {
                            // move back
                            trayObjectMoved.transform.position = new Vector3(trayObjectMoved.transform.position.x + maxXDiffOutOfTray, trayObjectMoved.transform.position.y, trayObjectMoved.transform.position.z);
                            tray.objectsToRight = new List<TrayObjectDraggable>(tray.objectsToRight.OrderBy(aBlock => aBlock.transform.position.x));
                            for (j = tray.objectsToRight.Count - 1; j >= 0; j--)
                            {
                                if (j - 1 >= 0 && tray.objectsToRight[j].boxCollider.bounds.Intersects(tray.objectsToRight[j - 1].boxCollider.bounds))
                                {
                                    xDiff = tray.objectsToRight[j - 1].boxCollider.bounds.max.x - tray.objectsToRight[j].boxCollider.bounds.min.x;
                                    tray.objectsToRight[j - 1].transform.position = new Vector3(tray.objectsToRight[j - 1].transform.position.x - xDiff, tray.objectsToRight[j - 1].transform.position.y, tray.objectsToRight[j - 1].transform.position.z);
                                }
                            }

                            break;
                        }
                    }
                }
            }

            tray.objectsToLeft.Clear();
            tray.objectsToRight.Clear();

            if (tray.currentDimensionObjectCapacity + trayObject.trayObject.dimensionFraction > tray.maxObjectCapacity)
            {
                NoSpaceEffect(tray);
            }

            if (movedObjects) tray.TrayUpdated();
        }

        private void NoSpaceEffect(Tray tray)
        {
            //Debug.Log ("currentCapacity + block.GetComponent<BlockDraggable> ().blockSize = " + (currentCapacity + block.GetComponent<BlockDraggable> ().blockSize));
            //Debug.Log ("maxCapacity = " + maxCapacityUint);
            for (int i = 0; i < tray.objects.Count; i++)
            {
                tray.objects[i].NoSpaceEffect();
            }
        }

        public void ResetSpaceEffect(Tray tray)
        {
            for (int i = 0; i < tray.objects.Count; i++)
            {
                tray.objects[i].ResetRotation();
            }
        }

        public void NotHovering(Tray tray)
        {
            ResetSpaceEffect(tray);
        }

        public bool CheckForRoom(TrayObject trayObject, Tray tray)
        {
            if (tray.currentDimensionObjectCapacity + trayObject.fraction > tray.maxObjectCapacity) return false;

            return true;
        }

        public bool CheckIfFull(Tray tray)
        {
            return tray.currentDimensionObjectCapacity >= tray.maxObjectCapacity;
        }

        public bool AddObject(TrayObjectDraggable trayObject, Tray tray)
        {
            Vector3 newPosition = new Vector3(trayObject.transform.position.x, tray.boxCollider.bounds.center.y + trayObject.yOffset, trayObject.transform.position.z);
            trayObject.transform.position = newPosition;
            //float maxXDiff;
            // can fit
            tray.currentDimensionObjectCapacity = 0;
            tray.currentObjectCapacity = 0;
            for (int i = 0; i < tray.objects.Count; i++)
            {
                tray.currentDimensionObjectCapacity += tray.objects[i].trayObject.dimensionFraction;
                tray.currentObjectCapacity += tray.objects[i].trayObject.fraction;
            }

            //Debug.Log ("1 <Tray> AddBlock -> currentCapacity = " + currentCapacity.ToString());
            //Debug.Log ("1 <Tray> AddBlock -> block.GetComponent<BlockDraggable> ().blockSize = " + block.GetComponent<BlockDraggable> ().fraction.ToString());
            //Debug.Log ("1 <Tray> AddBlock -> currentCapacity + block.GetComponent<BlockDraggable> ().blockSize = " + new Fraction(currentCapacity + block.GetComponent<BlockDraggable> ().fraction).ToString());
            if (tray.currentDimensionObjectCapacity + trayObject.trayObject.dimensionFraction <= tray.maxObjectCapacity)
            {
                tray.currentDimensionObjectCapacity += trayObject.trayObject.dimensionFraction;
                tray.currentObjectCapacity += trayObject.trayObject.fraction;
                //              Debug.Log ("<Tray> AddBlock -> block.GetComponent<BlockDraggable> ().blockSize = " + block.GetComponent<BlockDraggable> ().blockSize);
                //              Debug.Log ("1 <Tray> AddBlock -> currentCapacity = " + currentCapacity);

                tray.objects.Add(trayObject);

                Sweep(tray);

                //Debug.Log ("2 <Tray> AddBlock -> currentCapacity = " + currentCapacity);
                //              Debug.Log ("<Tray> AddBlock -> maxCapacity = " + maxCapacity);
                //              Debug.Log ("<Tray> AddBlock -> currentCapacity float = " + (float)currentCapacity);
                //float currentCapacityFloat = (float)currentCapacity;
                if (tray.currentDimensionObjectCapacity >= tray.maxObjectCapacity)
                {
                    TrayFull(tray);
                }

                TrayObjectAdded(tray);

                return true;
            }
            // can't fit
            else
            {
                Sweep(tray);

                return false;
            }
        }

        private void TrayObjectAdded(Tray tray)
        {
            tray.TrayObjectAdded();
        }

        protected virtual void TrayFull(Tray tray)
        {

        }

        protected virtual void TrayNotFull(Tray tray)
        {

        }

        protected virtual void TrayObjectRemoved(Tray tray)
        {
            tray.TrayObjectRemoved();
        }

        public void RemoveTrayObject(TrayObjectDraggable trayObject, Tray tray, bool sweep = true)
        {
            if (tray.objects.Remove(trayObject))
            {
                tray.currentDimensionObjectCapacity = 0;
                tray.currentObjectCapacity = 0;
                for (int i = 0; i < tray.objects.Count; i++)
                {
                    tray.currentDimensionObjectCapacity += tray.objects[i].trayObject.dimensionFraction;
                    tray.currentObjectCapacity += tray.objects[i].trayObject.fraction;
                }
                //Debug.Log ("<Tray> RemoveBlock -> currentCapacity = " + currentCapacity);
                if (sweep) Sweep(tray);

                TrayObjectRemoved(tray);
            }

            if (tray.currentDimensionObjectCapacity < tray.maxObjectCapacity)
                TrayNotFull(tray);
        }

        public void RemoveAllObjects(Tray tray, bool dispose = false)
        {
            for (int i = 0; i < tray.objects.Count; i++)
            {
                if (dispose) 
                {
                    tray.objects[i].Dispose();
                }
                else
                {
                    tray.objects[i].ResetParent();
                }
            }
            tray.currentDimensionObjectCapacity = 0;
            tray.currentObjectCapacity = 0;
            TrayNotFull(tray);
            tray.objects.Clear();
        }

        public void Sweep(Tray tray)
        {
            if (tray.objects.Count > 0)
            {
                switch(tray.justified)
                {
                    case Justified.None:
                        JustifiedNone(tray);
                        break;

                    case Justified.Left:
                        JustifiedSide(tray);
                        break;

                    case Justified.Right:
                        JustifiedSide(tray);
                        break;
                }
            }
        }

        private void JustifiedSide(Tray tray)
        {
            float minXDiffOutOfTray;
            float xDiff;
            tray.objects = new List<TrayObjectDraggable>(tray.objects.OrderBy(aObject => aObject.transform.position.x));
            List<TrayObjectDraggable> tempTrayObjects = new List<TrayObjectDraggable>(tray.objects);
            TrayObjectDraggable trayObject;
            TrayObjectDraggable lastTrayObject = trayObject = tempTrayObjects[0];
            for (int i = 0; i < tempTrayObjects.Count; i++)
            {
                trayObject = tempTrayObjects[i];
                if (i == 0)
                {
                    minXDiffOutOfTray = tray.boxCollider.bounds.min.x - trayObject.boxCollider.bounds.min.x;
                    trayObject.transform.position = new Vector3(trayObject.transform.position.x + minXDiffOutOfTray, tray.boxCollider.bounds.center.y + trayObject.yOffset, trayObject.transform.position.z);
                }
                else if (lastTrayObject != null)
                {
                    xDiff = lastTrayObject.boxCollider.bounds.max.x - trayObject.boxCollider.bounds.min.x;
                    trayObject.transform.position = new Vector3(trayObject.transform.position.x + xDiff, tray.boxCollider.bounds.center.y + trayObject.yOffset, trayObject.transform.position.z);                   
                }
                trayObject.UpdateSortOrder(i);
                lastTrayObject = trayObject;
            }

            //if (movedObjects) 
                tray.TrayUpdated();
        }

        private void JustifiedNone(Tray tray)
        {
            bool movedObjects = false;
            List<TrayObjectDraggable> tempTrayObjects;
            TrayObjectDraggable tempTrayObjectDraggable;
            bool foundIntersection = true;
            int maxAttempts = 10 * tray.objects.Count;
            int attempts = 0;
            float minXDiffOutOfTray;
            float maxXDiffOutOfTray;
            float xDiff;
            TrayObjectDraggable trayObject;
            TrayObjectDraggable tempTrayObject;

            while (foundIntersection && attempts < maxAttempts)
            {
                foundIntersection = false;
                attempts++;

                // left to right
                tray.objects = new List<TrayObjectDraggable>(tray.objects.OrderBy(aObject => aObject.transform.position.x));
                tempTrayObjects = new List<TrayObjectDraggable>(tray.objects);

                for (int i = 0; i < tray.objects.Count; i++)
                {
                    tempTrayObjectDraggable = tray.objects[i];
                    trayObject = tempTrayObjectDraggable;
                    // sortorder tempTrayObjectDraggable.transform.position = new Vector3(tempTrayObjectDraggable.transform.position.x, tempTrayObjectDraggable.transform.position.y, (-.1f * i));
                    tempTrayObjectDraggable.UpdateSortOrder(i);
                    tempTrayObjects.Remove(tempTrayObjectDraggable);

                    // too far to the left
                    minXDiffOutOfTray = tray.boxCollider.bounds.min.x - trayObject.boxCollider.bounds.min.x;
                    if (minXDiffOutOfTray > 0)
                    {
                        trayObject.transform.position = new Vector3(trayObject.transform.position.x + minXDiffOutOfTray, tray.boxCollider.bounds.center.y + trayObject.yOffset, trayObject.transform.position.z);
                        movedObjects = true;
                    }

                    // check intersection with ever other block
                    for (int j = 0; j < tempTrayObjects.Count; j++)
                    {
                        tempTrayObject = tempTrayObjects[j];
                        if (trayObject.boxCollider.bounds.Intersects(tempTrayObject.boxCollider.bounds))
                        {
                            foundIntersection = true;

                            xDiff = trayObject.boxCollider.bounds.max.x - tempTrayObject.boxCollider.bounds.min.x;
                            tempTrayObject.transform.position = new Vector3(tempTrayObject.transform.position.x + xDiff, tray.boxCollider.bounds.center.y + trayObject.yOffset, tempTrayObject.transform.position.z);
                            movedObjects = true;
                        }
                    }
                }

                // right to left
                tray.objects = new List<TrayObjectDraggable>(tray.objects.OrderBy(aBlock => aBlock.transform.position.x));
                tray.objects.Reverse();
                tempTrayObjects = new List<TrayObjectDraggable>(tray.objects);

                for (int i = 0; i < tray.objects.Count; i++)
                {
                    trayObject = tray.objects[i];
                    tempTrayObjects.Remove(tray.objects[i]);

                    // check if too far to the right
                    maxXDiffOutOfTray = tray.boxCollider.bounds.max.x - trayObject.boxCollider.bounds.max.x;
                    if (maxXDiffOutOfTray < 0)
                    {
                        trayObject.transform.position = new Vector3(trayObject.transform.position.x + maxXDiffOutOfTray, tray.boxCollider.bounds.center.y + trayObject.yOffset, trayObject.transform.position.z);
                        movedObjects = true;
                    }

                    // check intersection with ever other block
                    for (int j = 0; j < tempTrayObjects.Count; j++)
                    {
                        tempTrayObject = tempTrayObjects[j];
                        if (trayObject.boxCollider.bounds.Intersects(tempTrayObject.boxCollider.bounds))
                        {
                            foundIntersection = true;

                            // right side
                            xDiff = tempTrayObject.boxCollider.bounds.max.x - trayObject.boxCollider.bounds.min.x;
                            tempTrayObject.transform.position = new Vector3(tempTrayObject.transform.position.x - xDiff, tray.boxCollider.bounds.center.y + trayObject.yOffset, tempTrayObject.transform.position.z);
                            movedObjects = true;
                        }
                    }
                }
            }

            if (movedObjects) tray.TrayUpdated();
        }

        public void SortRenderOrder(Tray tray)
        {
            for (int i = 0; i < tray.objects.Count; i++)
            {
                tray.objects[i].UpdateSortOrder(i);
            }
        }
    }
}