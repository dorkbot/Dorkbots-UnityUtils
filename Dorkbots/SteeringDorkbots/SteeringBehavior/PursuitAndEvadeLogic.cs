using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class PursuitAndEvadeLogic : SteeringBehaviorWithTargetsLogic
    {
        public bool Evade = false;
        public float BrakingDistance = 3f;
        public Vector3 FutureTargetPosition { get; protected set; }
        
        private Vector3 _dir;
        
        protected override float CalculateSpeed()
        {
            if (Evade)
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
                FutureTargetPosition = Target.Position + Target.DesiredDir.normalized * Target.DesiredSpeed * Target.MaxSpeed;
                _dir = Evade ? Position - FutureTargetPosition : FutureTargetPosition - Position;

                return Target == null ? GetForward() : _dir.normalized;
            }
            return Vector3.zero;
        }
    }
}