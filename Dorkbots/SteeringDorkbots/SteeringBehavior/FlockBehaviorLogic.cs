using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class FlockBehaviorLogic : SeekAndFleeBehaviorLogic
    {
        public float NeighborhoodDistance = 5f;
        public float SeparationRadius = 2f;

        [Range(0.0f, 1.0f)] public float AligmentFactor = 0.25f;

        [Range(0.0f, 1.0f)] public float SeparationFactor = 0.5f;

        [Range(0.0f, 1.0f)] public float CohesionFactor = 0.25f;

        protected SteeringBehaviorLogic CurrentTarget;
        protected SteeringBehaviorLogic InitialTarget;

        private static List<FlockBehaviorLogic> _flocks = new List<FlockBehaviorLogic>();

        private List<FlockBehaviorLogic> _flocksNeighborhood = new List<FlockBehaviorLogic>();
        private FlockBehaviorLogic _localLeader = null;
        private float _distanceToTarget, _speed;

        public override void Init()
        {
            AddToFlock();
            base.Init();
        }
        
        public override void SetTarget(SteeringBehaviorLogic logic)
        {
            base.SetTarget(logic);
            InitialTarget = Target;
            CurrentTarget = InitialTarget;
        }

        public override void Update()
        {
            if (Target != null) _distanceToTarget = Vector3.Distance(Position, InitialTarget.Position);
            base.Update();
        }
        
        public void AddToFlock()
        {
            if (!_flocks.Contains(this))
            {
                _flocks.Add(this);
            }
        }
        
        public void RemoveFromFlock()
        {
            _flocks.Remove(this);
        }

        protected override void CalculateTargets()
        {
            base.CalculateTargets();
            InitialTarget = Target;
        }

        protected override float CalculateSpeed()
        {
            if (CurrentTarget == InitialTarget)
            {
                _speed = base.CalculateSpeed();
                return _speed;
            }
            else
            {
                _speed = _localLeader._speed;
                return _localLeader._speed;
            }
        }

        protected override Vector3 CalculateDirection()
        {
            if (Target == null) return Vector3.zero;
            
            Vector3 separation = Vector3.zero;
            Vector3 alineation = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 center = Vector3.zero;

            int neighbourCount = 0;
            _flocksNeighborhood.Clear();
            _flocksNeighborhood.Add(this);

            foreach (var flock in _flocks)
            {
                if (flock == this) continue;

                float dist = Vector3.Distance(flock.Position, Position);
                if (dist > NeighborhoodDistance || flock.Target != Target) continue;

                _flocksNeighborhood.Add(flock);
                neighbourCount++;

                if (dist < SeparationRadius)
                {
                    float n = 1 - dist / SeparationRadius;
                    separation += (Position - flock.Position).normalized * n;
                }

                alineation += (CurrentTarget?.Position ?? flock.Position) - flock.Position;
                //alineation += CurrentTarget.Position - flock.Position;

                center += flock.Position + alineation;
                //center += flock.Position + (CurrentTarget.Position - flock.Position);
            }

            _flocksNeighborhood.Sort((p1, p2) => p1._distanceToTarget.CompareTo(p2._distanceToTarget));

            if (_flocksNeighborhood[0] != this)
            {
                int index = _flocksNeighborhood.FindIndex((f1) => f1 == this);
                CurrentTarget = _flocksNeighborhood[0];
                _localLeader = _flocksNeighborhood[0];
            }
            else
            {
                CurrentTarget = InitialTarget;
            }

            if (neighbourCount == 0)
            {
                //this duplicates Flee Pursuit And Evade logic
                Vector3 futureTargetPosition = InitialTarget.Position + (InitialTarget.DesiredDir.normalized * InitialTarget.DesiredSpeed * InitialTarget.MaxSpeed);
                Vector3 direction;
                if (Flee)
                {
                    direction = Position - futureTargetPosition;
                }
                else
                {
                    direction = futureTargetPosition - Position;
                }

                return direction.normalized;
                //return InitialTarget.Position - Position;
            }

            center /= neighbourCount;
            cohesion = center - Position;

            return (separation.normalized * SeparationFactor + cohesion.normalized * CohesionFactor + alineation.normalized * AligmentFactor);
        }
    }
}