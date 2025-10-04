using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class FlockWithAvoidanceBehavior : FlockBehavior
    {
        [Header("Avoid Obstacle")] 
        [SerializeField] private bool showAvoidObstacleGizmos;

        [Range(0, 1)] 
        [SerializeField] private float verticalAngle = 1;
        [SerializeField] private int horizontalAccuracy = 0;
        [SerializeField] private LayerMask mask;
        [SerializeField] private float raycastDistance;
        [Range(0f, 1f)]
        [SerializeField] private float separation = 1f;
        [Range(1f, 100f)]
        [SerializeField] private float correctionFactor = 50f;

        private FlockWithAvoidanceBehaviorLogic _flockWithAvoidanceBehaviorLogic;

        protected override void Update()
        {
            base.Update();
            /*
             * Add wander logic
             * if target null, then check wander bool
             * instantiate/setup target, parent to this object's parent
             * when close, randomly move target within sphere
             */
        }
        
        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _flockWithAvoidanceBehaviorLogic.VerticalAngle = verticalAngle;
            _flockWithAvoidanceBehaviorLogic.HorizontalAccuracy = horizontalAccuracy;
            _flockWithAvoidanceBehaviorLogic.Mask = mask;
            _flockWithAvoidanceBehaviorLogic.RaycastDistance = raycastDistance;
            _flockWithAvoidanceBehaviorLogic.Separation = separation;
            _flockWithAvoidanceBehaviorLogic.CorrectionFactor = correctionFactor;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new FlockWithAvoidanceBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _flockWithAvoidanceBehaviorLogic = (FlockWithAvoidanceBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (_flockWithAvoidanceBehaviorLogic == null) return;
            if (showAvoidObstacleGizmos)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.forward.normalized * _flockWithAvoidanceBehaviorLogic.RaycastDistance);

                if (verticalAngle != 1)
                {
                    Gizmos.DrawRay(transform.position, (transform.forward * _flockWithAvoidanceBehaviorLogic.VerticalAngle + transform.up * (1 - _flockWithAvoidanceBehaviorLogic.VerticalAngle)).normalized * _flockWithAvoidanceBehaviorLogic.RaycastDistance * 2);
                    Gizmos.DrawRay(transform.position, (transform.forward * _flockWithAvoidanceBehaviorLogic.VerticalAngle - transform.up * (1 - _flockWithAvoidanceBehaviorLogic.VerticalAngle)).normalized * _flockWithAvoidanceBehaviorLogic.RaycastDistance * 2);
                }

                float subdivision = (90f / (float) horizontalAccuracy) / 100f;
                for (int i = 0; i < horizontalAccuracy; i++)
                {
                    Gizmos.DrawRay(transform.position, ((transform.forward * (subdivision * i)) + transform.right * (_flockWithAvoidanceBehaviorLogic.HorizontalAccuracy - i) * subdivision).normalized * _flockWithAvoidanceBehaviorLogic.RaycastDistance);
                    Gizmos.DrawRay(transform.position, ((transform.forward * (subdivision * i)) - transform.right * (_flockWithAvoidanceBehaviorLogic.HorizontalAccuracy - i) * subdivision).normalized * _flockWithAvoidanceBehaviorLogic.RaycastDistance);
                }
            }
        }
    }
}