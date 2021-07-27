using System.Collections.Generic;
using UnityEngine;
using Signals;
using Dorkbots.Fractions;

namespace Dorkbots.Tray
{
    public class Tray : MonoBehaviour 
	{
		[SerializeField] private float maxCapacity = 1;
        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private TrayController.Justified _justified = TrayController.Justified.None;
        public TrayController.Justified justified { get { return _justified; } }

        [HideInInspector] public List<TrayObjectDraggable> objects;
        [HideInInspector] public List<TrayObjectDraggable> objectsToLeft;
        [HideInInspector] public List<TrayObjectDraggable> objectsToRight;
        [HideInInspector] public BoxCollider2D boxCollider{ get; private set; }
        [HideInInspector] public Fraction currentDimensionObjectCapacity;
        [HideInInspector] public Fraction currentObjectCapacity;
        [HideInInspector] public Fraction maxObjectCapacity{ get; private set; }

        public Signal<Tray> objectUpdateSignal { get; private set; }
        public Signal<Tray> objectAddedSignal { get; private set; }
        public Signal<Tray> objectRemovedSignal { get; private set; }

        public TrayController trayController { get; private set; }

		protected virtual void Awake()
		{
			currentDimensionObjectCapacity = new Fraction (0, 1);
			currentObjectCapacity  = new Fraction (0, 1);
			maxObjectCapacity = new Fraction ((long)maxCapacity, (long)1);

            boxCollider = _boxCollider;

            objects = new List<TrayObjectDraggable> ();
            objectsToLeft = new List<TrayObjectDraggable> ();
            objectsToRight = new List<TrayObjectDraggable> ();

            objectAddedSignal = new Signal<Tray>();
			objectUpdateSignal = new Signal<Tray> ();
            objectRemovedSignal = new Signal<Tray>();
		}

        private void OnDestroy()
        {
            objectAddedSignal.Dispose();
            objectUpdateSignal.Dispose();
            objectRemovedSignal.Dispose();
        }

        public void Init(TrayController trayController)
        {
            this.trayController = trayController;
        }

        public void TrayObjectAdded()
		{
            objectAddedSignal.Dispatch(this);
			objectUpdateSignal.Dispatch (this);
		}

        public void TrayObjectRemoved()
        {
            objectRemovedSignal.Dispatch(this);
            objectUpdateSignal.Dispatch(this);
        }

        public void TrayUpdated()
        {
            objectUpdateSignal.Dispatch(this);
        }
	}
}