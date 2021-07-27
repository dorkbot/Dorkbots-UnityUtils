using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.Tray
{
    public class TrayObjectFactory : MonoBehaviour 
	{
		[SerializeField] private GameObject trayObjectPrefab;
        [SerializeField] private TrayObjectDraggableController trayObjectDraggableController;

		public List<TrayObjectDraggable> trayObjects { get; private set; }

        // Use this for initialization
		protected virtual void Awake () 
		{
            trayObjects = new List<TrayObjectDraggable> ();
		}

        public TrayObjectDraggable CreateTrayObject(Vector2 position)
        {
            GameObject trayObjectGO = Instantiate(trayObjectPrefab);
            trayObjectGO.transform.position = position;

            return SetupTrayObject(trayObjectGO.GetComponent<TrayObjectDraggable>());
        }

        public TrayObjectDraggable SetupTrayObject(TrayObjectDraggable trayObjectDraggable)
        {
            trayObjectDraggable.GetComponent<TrayObject>().InitFraction();

            trayObjectDraggableController.AddTrayObject(trayObjectDraggable);

            trayObjects.Add(trayObjectDraggable);

            return trayObjectDraggable;
        }

		public void ClearAllBlocks()
		{
            for (int i = 0; i < trayObjects.Count; i++)
            {
                trayObjects[i].Dispose();
            }

            trayObjects.Clear ();
		}
	}
}