using System.Collections.Generic;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public abstract class SteeringBehaviorWithTargetsLogic : SteeringBehaviorLogic
    {
        public bool MultipleTargets = false;
        public SteeringBehaviorLogic Target { protected set; get; }
        public readonly List<SteeringBehaviorLogic> Targets = new List<SteeringBehaviorLogic>();
        public float StopDistance = .1f;
        
        protected float TargetDistance;
        
        private int _currrentTargetIndex = 0;

        public override void Update()
        {
            CalculateTargets();
            if (Target != null) base.Update();
            
            // if (Target != null)
            // {
            //     CalculateTargets();
            //     base.Update(); 
            // }
        }
        
        protected override void Move()
        {
            if (Vector3.Distance(Position, Target.Position) > StopDistance)
            {
                base.Move();
            }
        }

        public virtual void SetTarget(SteeringBehaviorLogic logic)
        {
            Target = logic;
        }

        protected virtual void CalculateTargets()
        {
            if (MultipleTargets || Target == null)
            //if (MultipleTargets)
            {
                int closestIndex = -1;
                TargetDistance = Mathf.Infinity;
                for (int i = 0; i < Targets.Count; i++)
                {
                    var tempDistance = Vector3.Distance(Position, Targets[i].Position);
                    if (tempDistance < TargetDistance)
                    {
                        TargetDistance = tempDistance;
                        closestIndex = i;
                        //_currrentTargetIndex = i;
                    }
                }
                
                if (closestIndex > -1)
                {
                    _currrentTargetIndex = closestIndex;
                    Target = Targets[_currrentTargetIndex];
                }
                //Target = Targets[_currrentTargetIndex];
            }
        }
    }
}