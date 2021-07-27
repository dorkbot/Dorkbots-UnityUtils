using System.Collections;
using Dorkbots.MonoBehaviorUtils;
using UnityEngine;

namespace Dorkbots.CameraTools
{
    public class HideWhenCameraAtDistance : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float actionDistance = .1f;
        [SerializeField] private float delay = 0;

        private bool hiding = false;
        private Coroutine delayCoroutine;

        private void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (!hiding)
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }
                if (_camera != null && Vector3.Distance(transform.position, _camera.transform.position) <= actionDistance)
                {
                    hiding = true;
                    StartStopCoroutine.StartCoroutine(ref delayCoroutine, DelayEnumerator(), this);
                }
            }
        }

        private void OnDestroy()
        {
            StartStopCoroutine.StopCoroutine(ref delayCoroutine, this);
        }

        //
        private IEnumerator DelayEnumerator()
        {
            yield return new WaitForSeconds(delay);

            gameObject.SetActive(false);
        }
    }
}
