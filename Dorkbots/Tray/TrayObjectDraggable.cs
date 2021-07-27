using Signals;
using UnityEngine;

namespace Dorkbots.Tray
{
	[RequireComponent(typeof(TrayObject))]
	public class TrayObjectDraggable : MonoBehaviour
	{
        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private float _yOffset = 0;
        public float yOffset { get { return _yOffset; } }
        
		public Signal<TrayObjectDraggable> mouseUpSignal { get; private set; }
		public Signal<TrayObjectDraggable> mouseDownSignal { get; private set; }
        public Signal<TrayObjectDraggable> disposeSignal { get; private set; }

        [HideInInspector] public TrayObjectDraggableController.TrayObjectStates state;

        public TrayObject trayObject { get; private set; }
		public Bounds bounds { get{ return boxCollider.bounds; } }

		//protected BoxCollider2D[] trayColliders;
        public BoxCollider2D boxCollider{ get; private set; }

        [HideInInspector] public Tray containingTray;
        [HideInInspector] public Tray lastTray;
        [HideInInspector] public Tray trayCurrentlyOver;

        public Vector3 lastPosition { get; private set; }
        public TrayObjectDraggableController trayObjectDraggableController { get; private set; }

        private Transform startParent;

        private bool perform = true;

		void Awake()
		{
            startParent = gameObject.transform.parent;

            state = TrayObjectDraggableController.TrayObjectStates.NotInUse;

			trayObject = GetComponent<TrayObject> ();

			// set twice because encapsulating objects might need it before init is called.
            boxCollider = _boxCollider;

            lastPosition = new Vector3();
		}

		void OnMouseDown()
		{
            if (perform)
            {
                lastPosition = transform.position;
                mouseDownSignal.Dispatch(this);  
            }
		}

		void OnMouseUp() 
		{
            if (perform) mouseUpSignal.Dispatch (this);
		}

        void OnEnable()
        {
            perform = true;
        }

        void OnDisable()
        {
            perform = false;
        }

        public void InitTrayObjectDraggable(TrayObjectDraggableController trayObjectDraggableController)
        {
            this.trayObjectDraggableController = trayObjectDraggableController;
			mouseUpSignal = new Signal<TrayObjectDraggable> ();
			mouseDownSignal = new Signal<TrayObjectDraggable> ();
            disposeSignal = new Signal<TrayObjectDraggable>();

            state = TrayObjectDraggableController.TrayObjectStates.NotInUse;
			
            boxCollider = _boxCollider;
            trayObject = GetComponent<TrayObject>();
		}

		public void NoSpaceEffect()
		{
            trayObject.goForNoSpaceEffect.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.realtimeSinceStartup * 10) * 2); 
		}
			
		public void ResetRotation()
		{
            trayObject.goForNoSpaceEffect.transform.rotation = Quaternion.Euler(0, 0, 0); 
		}

		public void UpdateSortOrder(int order)
		{
            trayObject.renderSortingGroup.sortingOrder = order;
		}

        public void ResetParent()
        {
            gameObject.transform.SetParent(startParent);
        }

		public void Dispose()
		{
            disposeSignal.Dispatch(this);

            mouseUpSignal.Dispose();
            mouseDownSignal.Dispose();
            disposeSignal.Dispose();

			Destroy (gameObject);
		}

		protected void Hide()
		{
            //for (int i = 0; i < trayObject.spriteRenderer.Length; i++)
            //{
            //    trayObject.spriteRenderer[i].enabled = false;
            //}
		}
	}
}