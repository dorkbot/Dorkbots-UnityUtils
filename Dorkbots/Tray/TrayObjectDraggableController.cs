using UnityEngine;

namespace Dorkbots.Tray
{
    public class TrayObjectDraggableController : MonoBehaviour
    {
        [SerializeField] private string draggingLayer;
        [SerializeField] private string notDraggingLayer;
        [SerializeField] private Tray[] trays;

        public enum TrayObjectStates
        {
            Dragging,
            InTray,
            NotInTray,
            NotInUse
        }

        private TrayObjectDraggable draggingTrayObject;
        public TrayController trayController { get; private set; }

        private Tray lastTrayRemoved;

        private void Awake()
        {
            trayController = new TrayController();
            for (int i = 0; i < trays.Length; i++)
            {
                trays[i].Init(trayController);
            }
        }

        void Update()
        {
            if (draggingTrayObject != null)
            {
                if (draggingTrayObject.state == TrayObjectStates.Dragging)
                {
                    DraggingUpdate();
                }
                else if(draggingTrayObject.transform.position != draggingTrayObject.lastPosition)
                {
                    ObjectStartedDragging(draggingTrayObject);
                }
            }
        }

        public void AddTrayObject(TrayObjectDraggable trayObjectDraggable)
        {
            InitTrayObject(trayObjectDraggable);
        }

        private void InitTrayObject(TrayObjectDraggable trayObjectDraggable)
        {
            trayObjectDraggable.InitTrayObjectDraggable(this);
            //for (int i = 0; i < trayObjectDraggable.trayObject.spriteRenderer.Length; i++)
            //{
            //    trayObjectDraggable.trayObject.spriteRenderer[i].sortingLayerName = notDraggingLayer;
            //}
            trayObjectDraggable.trayObject.renderSortingGroup.sortingLayerName = notDraggingLayer;
            trayObjectDraggable.mouseUpSignal.Add(TrayObjectMouseUpHandler);
            trayObjectDraggable.mouseDownSignal.Add(TrayObjectMouseDownHandler);
            trayObjectDraggable.disposeSignal.Add(TrayObjectDisposeHandler);
        }

        // Handlers
        private void TrayObjectMouseUpHandler(TrayObjectDraggable trayObjectDraggable)
        {
            //Debug.Log("<TrayObjectDraggableController> TrayObjectMouseUpHandler");
            int i;
           
            //for (i = 0; i < trayObjectDraggable.trayObject.spriteRenderer.Length; i++)
            //{
            //    // sortorder trayObjectDraggable.trayObject.spriteRenderer[i].sortingOrder -= 1;...
            //    trayObjectDraggable.trayObject.spriteRenderer[i].sortingLayerName = notDraggingLayer;
            //}
            trayObjectDraggable.trayObject.renderSortingGroup.sortingOrder = 0;
            trayObjectDraggable.trayObject.renderSortingGroup.sortingLayerName = notDraggingLayer;

            if (trayObjectDraggable.state == TrayObjectStates.Dragging)
            {
                trayObjectDraggable.state = TrayObjectStates.NotInTray;

                bool overTray = false;
                bool addedToTray = false;

                for (i = 0; i < trays.Length; i++)
                {
                    if (draggingTrayObject.boxCollider.bounds.Intersects(trays[i].boxCollider.bounds))
                    {
                        trayController.NotHovering(trays[i]);
                        addedToTray = AddToTray(trays[i], trayObjectDraggable);
                        overTray = true;
                        break;
                    }
                }

                for (int j = i; j < trays.Length; j++)
                {
                    trayController.NotHovering(trays[j]);
                }

                if (!overTray)
                {
                    RemoveFromTray(trayObjectDraggable);
                }
                else
                {
                    if (!addedToTray) trayObjectDraggable.transform.position = trayObjectDraggable.lastPosition;
                }

                if (lastTrayRemoved != null) 
                {
                    trayController.Sweep(lastTrayRemoved);
                    lastTrayRemoved = null;
                }

            }
            else if (trayObjectDraggable.containingTray != null)
            {
                trayController.SortRenderOrder(trayObjectDraggable.containingTray);
            }

            draggingTrayObject = null;
        }

        private void TrayObjectMouseDownHandler(TrayObjectDraggable trayObjectDraggable)
        {
            //Debug.Log("<TrayObjectDraggableController> TrayObjectMouseDownHandler");
            trayObjectDraggable.transform.position = new Vector3(trayObjectDraggable.transform.position.x, trayObjectDraggable.transform.position.y, 0);
            //for (int i = 0; i < trayObjectDraggable.trayObject.spriteRenderer.Length; i++)
            //{
            //    // sortorder trayObjectDraggable.trayObject.spriteRenderer[i].sortingOrder += 1;...
            //    //trayObjectDraggable.trayObject.spriteRenderer[i].sortingLayerName = draggingLayer;
            //}
            trayObjectDraggable.trayObject.renderSortingGroup.sortingOrder = 1;
            trayObjectDraggable.trayObject.renderSortingGroup.sortingLayerName = draggingLayer;

            //trayObjectDraggable.state = TrayObjectStates.Dragging;
            //Tray oldTray = trayObjectDraggable.containingTray;
            draggingTrayObject = trayObjectDraggable;
            // move objects underneath 
            //if (oldTray != null) 
            //{
            //    if (draggingTrayObject.boxCollider.bounds.Intersects(oldTray.boxCollider.bounds))
            //    {
            //        //RemoveFromTray(trayObjectDraggable, false);
            //        draggingTrayObject.trayCurrentlyOver = oldTray;
            //        trayController.ObjectHover(draggingTrayObject, oldTray);
            //    }
            //    else
            //    {
            //        RemoveFromTray(trayObjectDraggable);
            //    }
            //}
        }

        private void ObjectStartedDragging(TrayObjectDraggable trayObjectDraggable)
        {
            trayObjectDraggable.state = TrayObjectStates.Dragging;
            Tray oldTray = trayObjectDraggable.containingTray;
            //draggingTrayObject = trayObjectDraggable;
            // move objects underneath 
            if (oldTray != null)
            {
                if (draggingTrayObject.boxCollider.bounds.Intersects(oldTray.boxCollider.bounds))
                {
                    RemoveFromTray(trayObjectDraggable);
                    //RemoveFromTray(trayObjectDraggable, false);
                    draggingTrayObject.trayCurrentlyOver = oldTray;
                    trayController.ObjectHover(draggingTrayObject, oldTray);
                }
                else
                {
                    RemoveFromTray(trayObjectDraggable);
                }
            }
        }

        private void TrayObjectDisposeHandler(TrayObjectDraggable trayObjectDraggable)
        {

        }

        private void DraggingUpdate()
        {
            draggingTrayObject.trayCurrentlyOver = null;
            for (int i = 0; i < trays.Length; i++)
            {
                if (draggingTrayObject.boxCollider.bounds.Intersects(trays[i].boxCollider.bounds))
                {
                    draggingTrayObject.trayCurrentlyOver = trays[i];
                    trayController.ObjectHover(draggingTrayObject, trays[i]);
                }
                else
                {
                    trayController.NotHovering(trays[i]);
                }
            }
        }

        public bool AddToTray(Tray tray, TrayObjectDraggable trayObjectDraggable)
        {
            return TrayUpdate(tray, trayObjectDraggable);
        }

        private void RemoveFromTray(TrayObjectDraggable trayObjectDraggable, bool sweep = true)
        {
            if (trayObjectDraggable.containingTray != null)
            {
                lastTrayRemoved = trayObjectDraggable.containingTray;
                trayController.RemoveTrayObject(trayObjectDraggable, trayObjectDraggable.containingTray, sweep);
                trayObjectDraggable.containingTray = null;
                trayObjectDraggable.ResetParent();
            }
        }

        private bool TrayUpdate(Tray tray, TrayObjectDraggable trayObjectDraggable, bool callNoSpace = true)
        {
            RemoveFromTray(trayObjectDraggable);

            bool added = trayController.AddObject(trayObjectDraggable, tray);
            if (added)
            {
                trayObjectDraggable.containingTray = tray;
                trayObjectDraggable.lastTray = tray;
                trayObjectDraggable.transform.SetParent(tray.transform);

                trayObjectDraggable.state = TrayObjectStates.InTray;
            }
            else if (callNoSpace)
            {
                NoSpace(trayObjectDraggable);
            }

            return added;
        }

        private void NoSpace(TrayObjectDraggable trayObjectDraggable)
        {
            if (draggingTrayObject.lastTray != null)
            {
                TrayUpdate(draggingTrayObject.lastTray, trayObjectDraggable, false);
            }
            else
            {
                trayObjectDraggable.transform.position = new Vector3(0, 0, -1);
                //for (int i = 0; i < trayObjectDraggable.trayObject.spriteRenderer.Length; i++)
                //{
                //    // sortorder trayObjectDraggable.trayObject.spriteRenderer[i].sortingOrder = 100;...
                //    // trayObjectDraggable.trayObject.spriteRenderer[i].sortingLayerName = draggingLayer;
                //}
                trayObjectDraggable.trayObject.renderSortingGroup.sortingOrder = 100;
                trayObjectDraggable.trayObject.renderSortingGroup.sortingLayerName = draggingLayer;

                trayObjectDraggable.transform.SetParent(trayObjectDraggable.transform.root);
            }
        }
    }
}