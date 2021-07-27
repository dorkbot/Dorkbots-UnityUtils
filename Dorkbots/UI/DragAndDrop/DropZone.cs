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
using System;
using System.Collections.Generic;
using System.Linq;
using Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dorkbots.UI.DragAndDrop
{
    [RequireComponent(typeof(LayoutGroup))]
    public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public enum OrientationTypes
        {
            horizontal,
            vertical
        }
        public OrientationTypes orientationType { get;  private set; }

        [Tooltip("Draggable Objects use this to avoid being added to certain Drop Zone types.")]
        [SerializeField] private int _type = 0;
        public int type { get { return _type; } }
        [Tooltip("The Draggable Objects that can't be added.")]
        [SerializeField] private int[] avoidDraggables;
        [Tooltip("The amount of Draggables that can fit. 0 means unlimited")]
        [SerializeField] private uint limit = 0;

        private LayoutGroup layoutGroup;

        public Signal<Draggable, DropZone> draggableDroppedSignal { get; private set; }
        public Signal<Draggable, DropZone> draggableRemovedSignal { get; private set; }
        public Signal<Draggable, DropZone> draggableHoveringSignal { get; private set; }
        public Signal<Draggable, DropZone> draggableExitSignal { get; private set; }
        public Signal<Draggable, DropZone> draggableBeginDragSignal { get; private set; }

        private void Awake()
        {
            layoutGroup = GetComponent<LayoutGroup>();
            if (layoutGroup is VerticalLayoutGroup)
            {
                orientationType = OrientationTypes.vertical;
            }
            else
            {
                orientationType = OrientationTypes.horizontal;
            }

            // remove any duplicates
            HashSet<int> hashSet = new HashSet<int>(avoidDraggables);
            avoidDraggables = hashSet.ToArray();

            draggableDroppedSignal = new Signal<Draggable, DropZone>();
            draggableRemovedSignal = new Signal<Draggable, DropZone>();
            draggableHoveringSignal = new Signal<Draggable, DropZone>();
            draggableExitSignal = new Signal<Draggable, DropZone>();
            draggableBeginDragSignal = new Signal<Draggable, DropZone>();
        }

        private void Start()
        {
            Draggable draggable;
            for (int i = 0; i < transform.childCount; i++)
            {
                draggable = transform.GetChild(i).GetComponent<Draggable>();
                if (draggable != null)
                {
                    draggable.currentDropZone = this;
                }
            }
        }

        private void OnDestroy()
        {
            draggableDroppedSignal.Dispose();
            draggableDroppedSignal = null;
            draggableRemovedSignal.Dispose();
            draggableRemovedSignal = null;
            draggableHoveringSignal.Dispose();
            draggableHoveringSignal = null;
            draggableExitSignal.Dispose();
            draggableExitSignal = null;
            draggableBeginDragSignal.Dispose();
            draggableBeginDragSignal = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null)
            {
                d.placeHolderParent = this.transform;
                d.currentDropZone = this;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null && d.placeHolderParent == this.transform)
            {
                d.placeHolderParent = d.parentToReturnTo;
                d.currentDropZone = null;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
            if (d != null)
            {
                bool add = false;
                if (!d.AvoidDropZone() )
                {
                    if (limit <= 0)
                    {
                        add = true;
                    }
                    else if (limit > GetDraggables(d).Length)
                    {
                        add = true;
                    }
                    else
                    {
                        add = false;
                    }
                }
                else
                {
                    add = false;
                }

                if (add)
                {
                    d.parentToReturnTo = this.transform;
                    d.currentDropZone = this;
                    d.AddedToDropZone();
                    draggableDroppedSignal.Dispatch(d, this);
                }
                else
                {
                    d.currentDropZone = null;
                }
            }
        }

        public void DraggableBeginDrag(Draggable draggable)
        {
            draggableBeginDragSignal.Dispatch(draggable, this);
        }

        public void DraggableRemoved(Draggable draggable)
        {
            draggableRemovedSignal.Dispatch(draggable, this);
        }

        public void DraggableHovering(Draggable draggable)
        {
            draggableHoveringSignal.Dispatch(draggable, this);
        }

        public void DraggableExited(Draggable draggable)
        {
            draggableExitSignal.Dispatch(draggable, this);
        }

        // called from the outside
        public void AddDraggable(Draggable draggable)
        {
            draggable.currentDropZone = this;
            bool add = false;
            if (!draggable.AvoidDropZone())
            {
                if (limit <= 0)
                {
                    add = true;
                }
                else if (limit > GetDraggables(draggable).Length)
                {
                    add = true;
                }
                else
                {
                    add = false;
                }
            }
            else
            {
                add = false;
            }

            if (add)
            {
                draggable.parentToReturnTo = this.transform;
                //draggable.currentDropZone = this;
                draggable.AddToParent(transform);
                draggable.AddedToDropZone();

                draggableDroppedSignal.Dispatch(draggable, this);
            }
            else
            {
                draggable.currentDropZone = null;
            }

            //draggable.parentToReturnTo = this.transform;
            //draggable.currentDropZone = this;
            //draggable.AddToParent(transform);
            //draggable.AddedToDropZone();
        }

        public bool AvoidDraggle(int type)
        {
            return (Array.IndexOf(avoidDraggables, type) > -1);
        }

        public void EmptyAndDestroy()
        {
            int childCount = transform.childCount;
            Transform[] children = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }
            for (int i = 0; i < children.Length; i++)
            {
                Destroy(children[i].gameObject);
            }
        }

        public Draggable[] GetDraggables(Draggable ignoreDraggable = null)
        {
            int childCount = transform.childCount;
            List<Draggable> draggables = new List<Draggable>();

            Draggable draggable;
            for (int i = 0; i < childCount; i++)
            {
                draggable = transform.GetChild(i).GetComponent<Draggable>();
                if (draggable != null && draggable != ignoreDraggable) draggables.Add(draggable);
            }

            return draggables.ToArray();
        }
    }
}