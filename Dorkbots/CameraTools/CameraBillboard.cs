using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class CameraBillboard : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private float zRotation;

        private void Awake()
        {
            zRotation = transform.rotation.z;
        }

        void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        void Update()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            if (_camera != null)
            {
                transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            }
        }
    }
}