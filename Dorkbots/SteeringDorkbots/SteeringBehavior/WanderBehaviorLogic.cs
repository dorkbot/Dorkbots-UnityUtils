using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class WanderBehaviorLogic : SeekAndFleeBehaviorLogic
    {
        public float DistanceSpawnTarget = 6f;
        public float DistanceToCalculateNewTarget = 3f;
        public float AreaToSpawnRadius = 3f;
        
        private float _randomX, _randomY, _randomZ;

        public override void Update()
        {
            if (Target != null && TargetDistance < DistanceToCalculateNewTarget)
            {
                _randomX = Random.Range(-AreaToSpawnRadius, AreaToSpawnRadius);
                _randomY = Random.Range(-AreaToSpawnRadius, AreaToSpawnRadius);
                _randomZ = Random.Range(-AreaToSpawnRadius, AreaToSpawnRadius);
                Target.UpdatePositionAndRotation(Position + (GetForward() * DistanceSpawnTarget) + new Vector3(_randomX, _randomY, _randomZ), Target.Rotation);
            }

            base.Update();
        }

        protected override void CalculateTargets()
        {

        }
    }
}