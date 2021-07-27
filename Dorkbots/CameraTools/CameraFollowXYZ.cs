using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class CameraFollowXYZ : MonoBehaviour
    {
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;
        [SerializeField] private bool followZ = true;
        public GameObject objectToFollow;
        public float speed = 2.0f;

        void Update()
        {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = this.transform.position;
            if (followX) position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);
            if (followY) position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
            if (followZ) position.z = Mathf.Lerp(this.transform.position.z, objectToFollow.transform.position.z, interpolation);

            this.transform.position = position;
        }
    }
}