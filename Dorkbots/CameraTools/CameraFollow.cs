using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothing = 5f;

        private Vector3 offset;

        // Use this for initialization
        void Start()
        {
            offset = transform.position - target.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 targetCamPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }

}