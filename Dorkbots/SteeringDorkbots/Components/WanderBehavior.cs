using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class WanderBehavior : SeekAndFleeBehavior
    {
        [Header("Wander")] 
        [SerializeField] private bool showWanderGizmos;
        
        [SerializeField] private float distanceSpawnTarget = 6f;
        [SerializeField] private float distanceToCalculateNewTarget = 3f;
        [SerializeField] private float areaToSpawnRadius = 3f;

        private WanderBehaviorLogic _wanderBehaviourLogic;

        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _wanderBehaviourLogic.DistanceSpawnTarget = distanceSpawnTarget;
            _wanderBehaviourLogic.DistanceToCalculateNewTarget = distanceToCalculateNewTarget;
            _wanderBehaviourLogic.AreaToSpawnRadius = areaToSpawnRadius;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new WanderBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _wanderBehaviourLogic = (WanderBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showWanderGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position + (transform.forward * distanceSpawnTarget),
                    areaToSpawnRadius);
            }
        }
    }
}