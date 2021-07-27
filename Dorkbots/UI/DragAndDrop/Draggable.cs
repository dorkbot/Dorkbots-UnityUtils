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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;
using Signals;

namespace Dorkbots.UI.DragAndDrop
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(CanvasGroup))]
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Tooltip("Used by Drop Zones so they can restrict types.")]
        [SerializeField] private int _type = 0;
        public int type { get { return _type; } }
        [Tooltip("The Drop Zones that this object can't be added to.")]
        [SerializeField] private int[] avoidDropZones;

        [HideInInspector] public Transform parentToReturnTo = null;
        [HideInInspector] public Transform placeHolderParent = null;
        [HideInInspector] public DropZone currentDropZone = null;
        // the last drop zone the draggable object lived in
        private DropZone lastDropZone = null;
        public int currentIndex { get; private set; }
        public int lastIndex { get; private set; }

        // used to put in drop zones that the draggable object is hovering over
        private GameObject placeHolder = null;
        // element left in the last drop zone
        private GameObject standIn = null;
        private LayoutElement layoutElement;
        private CanvasGroup canvasGroup;

        public Signal<Draggable, DropZone> addedToDropZoneSignal { get; private set; }
        public Signal<Draggable, DropZone> removeFromDropZoneSignal { get; private set; }

        private void Awake()
        {
            // remove any duplicates
            HashSet<int> hashSet = new HashSet<int>(avoidDropZones);
            avoidDropZones = hashSet.ToArray();

            addedToDropZoneSignal = new Signal<Draggable, DropZone>();
            removeFromDropZoneSignal = new Signal<Draggable, DropZone>();

            layoutElement = GetComponent<LayoutElement>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            addedToDropZoneSignal.Dispose();
            addedToDropZoneSignal = null;
            removeFromDropZoneSignal.Dispose();
            removeFromDropZoneSignal = null;

            // notify DropZone that this Draggable Object was hovering over
            DropZone standInHolderDropZone = null;
            if (standIn != null)
            {
                standInHolderDropZone = standIn.transform.parent.GetComponent<DropZone>();
                standInHolderDropZone.DraggableRemoved(this);
            }

            if (placeHolder != null)
            {
                DropZone placeHolderDropZone = placeHolder.transform.parent.GetComponent<DropZone>();
                if (standInHolderDropZone != placeHolderDropZone)
                { 
                    placeHolderDropZone.DraggableExited(this);
                }
            }

            Destroy(standIn);
            standIn = null;
            Destroy(placeHolder);
            placeHolder = null;

            //if (lastDropZone != null)
            //{
            //    lastDropZone.DraggableRemoved(this);
            //}

            //if (currentDropZone != null && lastDropZone != currentDropZone)
            //{
            //    currentDropZone.DraggableExited(this);
            //}
            parentToReturnTo = null;
            placeHolderParent = null;
            currentDropZone = null;
            lastDropZone = null;
            layoutElement = null;
            canvasGroup = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            currentIndex = lastIndex = transform.GetSiblingIndex();

            placeHolder = CreateHolder(transform.GetSiblingIndex(), transform.parent);

            parentToReturnTo = this.transform.parent;
            placeHolderParent = parentToReturnTo;
            // move it up
            transform.SetParent(this.transform.root);
            //transform.SetParent(this.transform.parent.parent);

            canvasGroup.blocksRaycasts = false;

            if (currentDropZone != null)
            {
                lastDropZone = currentDropZone;
                //removeFromDropZoneSignal.Dispatch(this, currentDropZone);
                currentDropZone.DraggableBeginDrag(this);
                // keep dropZone set
                //dropZone = null;
            }
        }

        public void OnDrag(PointerEventData eventData)
        { 
            transform.position = eventData.position;

            if (!AvoidDropZone())
            {
                if (placeHolder.transform.parent != placeHolderParent)
                {
                    if (currentDropZone == lastDropZone && standIn != null)
                    {
                        // remove standIn if hovering over the dropzone that the draggable object was last in
                        Destroy(standIn);
                        standIn = null;
                    }

                    DropZone lastHoveringDropZone = null;
                    if (placeHolder.transform.parent != placeHolderParent)
                    {
                        // Hovering over new DropZone
                        lastHoveringDropZone = placeHolder.transform.parent.GetComponent<DropZone>();
                    }

                    placeHolder.transform.SetParent(placeHolderParent);

                    if (standIn == null && currentDropZone != lastDropZone && lastDropZone != null)
                    {
                        // put a place holder in the last drop zone the draggable object live in
                        standIn = CreateHolder(lastIndex, lastDropZone.transform);
                    }

                    if (lastHoveringDropZone != null)
                    {
                        // notify DropZones after standIn and placeHolder have been updated.
                        currentDropZone.DraggableHovering(this);
                        lastHoveringDropZone.DraggableExited(this);
                    }
                }

                currentIndex = placeHolderParent.childCount;

                bool shift = false;

                for (int i = 0; i < placeHolderParent.childCount; i++)
                {
                    if (currentDropZone.orientationType == DropZone.OrientationTypes.vertical)
                    {
                        if (this.transform.position.y > placeHolderParent.GetChild(i).position.y) shift = true;
                    }
                    else
                    {
                        if (this.transform.position.x < placeHolderParent.GetChild(i).position.x) shift = true;
                    }

                    if (shift)
                    {
                        currentIndex = i;

                        if (placeHolder.transform.GetSiblingIndex() < currentIndex)
                            currentIndex--;

                        break;
                    }
                }

                placeHolder.transform.SetSiblingIndex(currentIndex);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (placeHolder != null)
            {
                Destroy(standIn);
                standIn = null;

                AddToParent(parentToReturnTo);
                transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());

                // placeHolder was left in a DropZone the Draggable object was hovering over.
                if (placeHolder.transform.parent != parentToReturnTo) placeHolder.transform.parent.GetComponent<DropZone>().DraggableExited(this);
                Destroy(placeHolder);
                placeHolder = null;

                if (lastDropZone != null)
                {
                    // new parent
                    if (lastDropZone.transform != parentToReturnTo)
                    {
                        removeFromDropZoneSignal.Dispatch(this, currentDropZone);
                        lastDropZone.DraggableRemoved(this);
                    }
                }
            }
        }

        public void SendBack()
        {
            if (lastDropZone != null)
            {
                currentDropZone = lastDropZone;
                parentToReturnTo = lastDropZone.transform;
                placeHolderParent = parentToReturnTo;
            }

            AddToParent(parentToReturnTo);
            transform.SetSiblingIndex(lastIndex);

            Destroy(standIn);
            standIn = null;

            Destroy(placeHolder);
            placeHolder = null;
        }

        public void AddToParent(Transform newParent)
        {
            transform.SetParent(newParent);
            canvasGroup.blocksRaycasts = true;
        }

        public bool AvoidDropZone()
        {
            if (currentDropZone != null)
            {
                if (Array.IndexOf(avoidDropZones, currentDropZone.type) > -1)
                {
                    return true;
                }

                return currentDropZone.AvoidDraggle(type);
            }
            else
            {
                return true;
            }
        }

        public void AddedToDropZone()
        {
            addedToDropZoneSignal.Dispatch(this, currentDropZone);
        }

        private GameObject CreateHolder(int siblingIndex, Transform parent)
        {
            GameObject newGameObject = new GameObject();
            newGameObject.transform.SetParent(parent);
            LayoutElement le = newGameObject.AddComponent<LayoutElement>();
            le.preferredWidth = layoutElement.preferredWidth;
            le.preferredHeight = layoutElement.preferredHeight;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            newGameObject.transform.SetSiblingIndex(siblingIndex);

            return newGameObject;
        }
    }
}