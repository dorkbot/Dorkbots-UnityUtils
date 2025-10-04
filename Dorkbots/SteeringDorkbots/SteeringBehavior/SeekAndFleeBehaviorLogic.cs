using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class SeekAndFleeBehaviorLogic : SteeringBehaviorWithTargetsLogic
    {
        public bool Flee = false;
        public float BrakingDistance = 3f;
        
        private Vector3 _dir;

        protected override float CalculateSpeed()
        {
            if (Flee)
            {
                return 1;
            }
            else
            {
                if (Target == null)
                    return 0;
                else
                {
                    TargetDistance = Vector3.Distance(Position, Target.Position); 
                    if (TargetDistance < BrakingDistance)
                        return TargetDistance / BrakingDistance;
                    else
                        return 1;
                }
            }
        }

        protected override Vector3 CalculateDirection()
        {
            if (Target != null)
            {
                _dir = Flee ? Position - Target.Position : Target.Position - Position;
                return Target == null ? GetForward() : _dir.normalized;
            }
            return Vector3.zero;
        }
    }
}