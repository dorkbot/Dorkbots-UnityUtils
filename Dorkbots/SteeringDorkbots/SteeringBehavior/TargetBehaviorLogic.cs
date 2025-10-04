using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class TargetBehaviorLogic : SteeringBehaviorLogic
    {
        private Vector3 _lastPosition;
        
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Only move using this method
        /// </summary>
        /// <param name="position"></param>
        public override void UpdatePosition(Vector3 position)
        {
            _lastPosition = Position;
            base.UpdatePosition(position);
        }
        
        protected override void Move()
        {
            
        }

        protected override void Rotate()
        {
            
        }
        
        protected override float CalculateSpeed()
        {
            return Vector3.Distance(_lastPosition, Position);
        }

        protected override Vector3 CalculateDirection()
        {
            return GetForward();
        }
    }
}