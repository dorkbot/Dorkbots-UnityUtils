using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class ZoomCameraMouseWheel : MonoBehaviour
    {
        [SerializeField] private float zoomSpeed = .5f;
        [SerializeField] private float fovMin;
        [SerializeField] private float fovMax;

        private Camera myCamera;

        // Use this for initialization
        void Start()
        {
            myCamera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                myCamera.fieldOfView += zoomSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                myCamera.fieldOfView -= zoomSpeed;
            }

            myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
        }
    }
}