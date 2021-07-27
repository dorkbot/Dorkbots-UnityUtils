/*
* Author: Dayvid jones
* http://www.dayvid.com
* Copyright(c) Superhero Robot 2018
* http://www.superherorobot.com
* Managed by Dorkbots
* http://www.dorkbots.com/
* Version: 1
*
* Licence Agreement
*
* You may distribute and modify this class freely, provided that you leave this header intact,
* and add appropriate headers to indicate your changes.Credit is appreciated in applications
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
using System.Collections;
using Dorkbots.MonoBehaviorUtils;
using Signals;
using UnityEngine;
using UnityEngine.UI;

namespace Dorkbots.UI.DragAndDrop
{
    public class ResizableDraggableDropZone : MonoBehaviour
    {
        [SerializeField] private DropZone dropZone;
        [SerializeField] private LayoutElement layoutElement;

        public Signal<DropZone> dropZoneEmptySignal { get; private set; }
        public Signal<DropZone> dropZoneFilledSignal { get; private set; }

        private LayoutGroup layoutGroup;
        private float preferredSize;
        private float spacing;
        private float layoutNewSize;
        private bool layoutGroupVertical = false;

        private Coroutine waitUntilEndOfFrameCoroutine;

        private void Awake()
        {
            dropZoneEmptySignal = new Signal<DropZone>();
            dropZoneFilledSignal = new Signal<DropZone>();
        }

        // Start is called before the first frame update
        void Start()
        {
            layoutGroup = dropZone.GetComponent<LayoutGroup>();

            StartStopCoroutine.StartCoroutine(ref waitUntilEndOfFrameCoroutine, SetupEnumerator(), this);
        }

        private void OnDestroy()
        {
            dropZoneEmptySignal.Dispose();
            dropZoneFilledSignal.Dispose();
        }

        private void SizeLayoutGroup(bool useDraggables)
        {
            layoutNewSize = 0;

            if (useDraggables)
            {
                Draggable[] draggables = dropZone.GetDraggables();
                for (int i = 0; i < draggables.Length; i++)
                {
                    if (draggables[i] != null)
                    {
                        if (layoutGroupVertical)
                        {
                            layoutNewSize += dropZone.transform.GetChild(i).GetComponent<LayoutElement>().preferredHeight;
                        }
                        else
                        {
                            layoutNewSize += dropZone.transform.GetChild(i).GetComponent<LayoutElement>().preferredWidth;
                        }
                        // add Spacing
                        if (i > 0) layoutNewSize += spacing;
                    }
                }
            }
            else
            {
                int childCount = dropZone.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    if (layoutGroupVertical)
                    {
                        layoutNewSize += dropZone.transform.GetChild(i).GetComponent<LayoutElement>().preferredHeight;
                    }
                    else
                    {
                        layoutNewSize += dropZone.transform.GetChild(i).GetComponent<LayoutElement>().preferredWidth;
                    }
                    // don't add spacing for first draggable
                    if (i > 0) layoutNewSize += spacing;
                }
            }

            if (layoutNewSize <= 0)
            {
                layoutNewSize = preferredSize;

                dropZoneEmptySignal.Dispatch(dropZone);
            }
            else
            {
                if (layoutGroupVertical)
                {
                    layoutNewSize += layoutGroup.padding.top;
                    layoutNewSize += layoutGroup.padding.bottom;
                }
                else
                {
                    layoutNewSize += layoutGroup.padding.left;
                    layoutNewSize += layoutGroup.padding.right;
                }

                dropZoneFilledSignal.Dispatch(dropZone);
            }

            if (layoutGroupVertical)
            {
                layoutElement.preferredHeight = layoutNewSize;
            }
            else
            {
                layoutElement.preferredWidth = layoutNewSize;
            }
        }

        private IEnumerator SetupEnumerator()
        {
            yield return new WaitForEndOfFrame();

            if (layoutGroup is VerticalLayoutGroup)
            {
                layoutGroupVertical = true;
                preferredSize = layoutElement.preferredHeight;
                VerticalLayoutGroup verticalLayoutGroup = (VerticalLayoutGroup)layoutGroup;
                spacing = verticalLayoutGroup.spacing;
            }
            else
            {
                layoutGroupVertical = false;
                preferredSize = layoutElement.preferredWidth;
                HorizontalLayoutGroup horizontalLayoutGroup = (HorizontalLayoutGroup)layoutGroup;
                spacing = horizontalLayoutGroup.spacing;
            }

            SizeLayoutGroup(true);

            dropZone.draggableDroppedSignal.Add(DraggableAddedChangeHandler);
            dropZone.draggableRemovedSignal.Add(DraggableRemovedChangeHandler);
            dropZone.draggableHoveringSignal.Add(DraggableEnterHandler);
            dropZone.draggableExitSignal.Add(DraggableExitHandler);
        }

        private IEnumerator WaitUntilEndOfFrameEnumerator(bool useDraggables)
        {
            yield return new WaitForEndOfFrame();

            SizeLayoutGroup(useDraggables);
        }

        // HANDLERS
        private void DraggableAddedChangeHandler(Draggable draggable, DropZone dropZone)
        {
            //Debug.Log("DraggableAddedChangeHandler");
            StartStopCoroutine.StartCoroutine(ref waitUntilEndOfFrameCoroutine, WaitUntilEndOfFrameEnumerator(true), this);
        }

        private void DraggableRemovedChangeHandler(Draggable draggable, DropZone dropZone)
        {
            //Debug.Log("DraggableRemovedChangeHandler");
            StartStopCoroutine.StartCoroutine(ref waitUntilEndOfFrameCoroutine, WaitUntilEndOfFrameEnumerator(true), this);
        }

        private void DraggableEnterHandler(Draggable draggable, DropZone dropZone)
        {
            //Debug.Log("DraggableEnterHandler");
            StartStopCoroutine.StartCoroutine(ref waitUntilEndOfFrameCoroutine, WaitUntilEndOfFrameEnumerator(false), this);
        }

        private void DraggableExitHandler(Draggable draggable, DropZone dropZone)
        {
            //Debug.Log("DraggableExitHandler");
            StartStopCoroutine.StartCoroutine(ref waitUntilEndOfFrameCoroutine, WaitUntilEndOfFrameEnumerator(false), this);
        }
    }
}