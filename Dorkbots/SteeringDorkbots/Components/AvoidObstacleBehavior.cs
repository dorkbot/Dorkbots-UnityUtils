using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class AvoidObstacleBehavior : SeekAndFleeBehavior
    {
        [Header("Avoid Obstacle")] 
        [SerializeField] private bool showAvoidObstacleGizmos;

        [Range(0, 1)] 
        [SerializeField] private float verticalAngle = 1;
        [SerializeField] private int horizontalAccuracy = 6;
        [SerializeField] private LayerMask mask;
        [SerializeField] private float raycastDistance = 2.5f;
        [Range(0f, 1f)] 
        [SerializeField] private float separation = 1f;
        [Range(1f, 100f)] 
        [SerializeField] private float correctionFactor = 60f;

        private AvoidObstacleBehaviorLogic _avoidObstacleBehaviorLogic;
        
        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _avoidObstacleBehaviorLogic.VerticalAngle = verticalAngle;
            _avoidObstacleBehaviorLogic.HorizontalAccuracy = horizontalAccuracy;
            _avoidObstacleBehaviorLogic.Mask = mask;
            _avoidObstacleBehaviorLogic.RaycastDistance = raycastDistance;
            _avoidObstacleBehaviorLogic.Separation = separation;
            _avoidObstacleBehaviorLogic.CorrectionFactor = correctionFactor;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new AvoidObstacleBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _avoidObstacleBehaviorLogic = (AvoidObstacleBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showAvoidObstacleGizmos)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.forward.normalized * raycastDistance);

                if (verticalAngle != 1)
                {
                    Gizmos.DrawRay(transform.position,
                        (transform.forward * verticalAngle + transform.up * (1 - verticalAngle)).normalized *
                        raycastDistance * 2);
                    Gizmos.DrawRay(transform.position,
                        (transform.forward * verticalAngle - transform.up * (1 - verticalAngle)).normalized *
                        raycastDistance * 2);
                }

                float subdivision = (90f / (float) horizontalAccuracy) / 100f;
                for (int i = 0; i < horizontalAccuracy; i++)
                {
                    Gizmos.DrawRay(transform.position,
                        ((transform.forward * (subdivision * i)) +
                         transform.right * (horizontalAccuracy - i) * subdivision).normalized * raycastDistance);
                    Gizmos.DrawRay(transform.position,
                        ((transform.forward * (subdivision * i)) -
                         transform.right * (horizontalAccuracy - i) * subdivision).normalized * raycastDistance);
                }
            }
        }
    }
}