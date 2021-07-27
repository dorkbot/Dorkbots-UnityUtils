using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class ZoomOrthoCameraMouseWheel : MonoBehaviour
    {
        [SerializeField] private float zoomSpeed = .5f;
        [SerializeField] private float orthographicSizeMin = 5f;
        [SerializeField] private float orthographicSizeMax = 15.5f;
        [SerializeField] private Camera _camera;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _camera.orthographicSize += zoomSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _camera.orthographicSize -= zoomSpeed;
            }

            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }

        public void ZoomOutToMax()
        {
            _camera.orthographicSize = orthographicSizeMax;
        }

        public void ZoomInToMin()
        {
            _camera.orthographicSize = orthographicSizeMin;
        }
    }
}
