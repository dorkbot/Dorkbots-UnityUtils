using System;
using System.Collections.Generic;
using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public abstract class SteeringBehaviorWithTargets : SteeringBehavior
    {
        [Header("Targets to Steering")] 
        [SerializeField] private bool showTargetsGizmos;
        
        [Tooltip("The distance from the target to stop")]
        [SerializeField] private float stopDistance = .1f;
        [SerializeField] protected bool multipleTargets = false;
        [SerializeField] protected TargetBehavior target;
        [SerializeField] protected List<TargetBehavior> targets;

        public List<TargetBehavior> Targets => _targets;
        
        public event Action<TargetBehavior> TargetAddedAction;
        public event Action<TargetBehavior> TargetRemovedAction;
        
        private SteeringBehaviorWithTargetsLogic _steeringBehaviorWithTargetsLogic;
        private int _targetsReady = 0;
        private readonly List<TargetBehavior> _targets = new List<TargetBehavior>();

        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _steeringBehaviorWithTargetsLogic.MultipleTargets = multipleTargets;
            _steeringBehaviorWithTargetsLogic.StopDistance = stopDistance;
        }
        
        protected override void InitLogic()
        {
            _steeringBehaviorWithTargetsLogic = (SteeringBehaviorWithTargetsLogic) SteeringBehaviorLogic;
            
            TargetBehavior targetBehavior;
            for (int i = 0; i < targets.Count; i++)
            {
                targetBehavior = targets[i];
                
                if (targetBehavior == null) continue;
                
                _targets.Add(targetBehavior);
                if (targetBehavior.SteeringBehaviorLogic != null)
                {
                    TargetLogicInstantiatedHandler(targetBehavior.SteeringBehaviorLogic);
                }
                else
                {
                    targetBehavior.LogicInstantiatedAction += TargetLogicInstantiatedHandler;
                }
            }

            if (target != null)
            {
                if (!targets.Contains(target))
                {
                    target.TargetDisarmedAction += TargetDisarmedHandler;
                }
                
                if (target.SteeringBehaviorLogic != null)
                {
                    _steeringBehaviorWithTargetsLogic.SetTarget(this.target.SteeringBehaviorLogic);
                }
                else
                {
                    target.LogicInstantiatedAction += logic =>
                    {
                        _steeringBehaviorWithTargetsLogic.SetTarget(logic);
                    };
                }
            }
            
            base.InitLogic();
        }

        public void AddTarget(TargetBehavior target)
        {
            if (!_targets.Contains(target))
            {
                if (target.SteeringBehaviorLogic != null)
                {
                    AddedTargetLogicInstantiatedHandler(target.SteeringBehaviorLogic);
                }
                else
                {
                    target.LogicInstantiatedAction += AddedTargetLogicInstantiatedHandler;
                }
                _targets.Add(target);
                TargetAddedAction?.Invoke(target);
            }
        }

        private void AddedTargetLogicInstantiatedHandler(SteeringBehaviorLogic logic)
        {
            _steeringBehaviorWithTargetsLogic.Targets.Add(logic);
        }

        public void RemoveTarget(TargetBehavior target)
        {
            if (!_targets.Contains(target)) return;
            if (target.SteeringBehaviorLogic == _steeringBehaviorWithTargetsLogic.Target) _steeringBehaviorWithTargetsLogic.SetTarget(null);
            _targets.Remove(target);
            _steeringBehaviorWithTargetsLogic.Targets.Remove(target.SteeringBehaviorLogic);
            TargetRemovedAction?.Invoke(target);
        }
        
        private void TargetArmedHandler(TargetBehavior targetBehavior)
        {
            if (!_steeringBehaviorWithTargetsLogic.Targets.Contains(targetBehavior.SteeringBehaviorLogic)) _steeringBehaviorWithTargetsLogic.Targets.Add(targetBehavior.SteeringBehaviorLogic);
            if (!_targets.Contains(targetBehavior)) _targets.Add(targetBehavior);
        }
        
        private void TargetDisarmedHandler(TargetBehavior targetBehavior)
        {
            if (targetBehavior == target) target = null;
            if(_steeringBehaviorWithTargetsLogic.Target == targetBehavior.SteeringBehaviorLogic) _steeringBehaviorWithTargetsLogic.SetTarget(null);
            _steeringBehaviorWithTargetsLogic.Targets.Remove(targetBehavior.SteeringBehaviorLogic);
            _targets.Remove(targetBehavior);
        }
        
        private void TargetLogicInstantiatedHandler(SteeringBehaviorLogic logic)
        {
            _targetsReady++;
            if (_targetsReady == targets.Count) //we do this to preserve the order of the Targets
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    TargetBehavior targetBehavior = targets[i];
                    targetBehavior.TargetArmedAction += TargetArmedHandler;
                    targetBehavior.TargetDisarmedAction += TargetDisarmedHandler;
                    if (targetBehavior.Armed) _steeringBehaviorWithTargetsLogic.Targets.Add(targetBehavior.SteeringBehaviorLogic);
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showTargetsGizmos)
            {
                if (!multipleTargets && target != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * Vector3.Distance(transform.position, target.transform.position));
                }
                else if (multipleTargets && _targets.Count > 0)
                {
                    for (int i = 0; i < _targets.Count; i++)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawRay(transform.position, (_targets[i].transform.position - transform.position).normalized * Vector3.Distance(transform.position, _targets[i].transform.position));
                    }

                    if (_steeringBehaviorWithTargetsLogic != null && _steeringBehaviorWithTargetsLogic.Target != null)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay(transform.position, (_steeringBehaviorWithTargetsLogic.Target.Position - transform.position).normalized * Vector3.Distance(transform.position, _steeringBehaviorWithTargetsLogic.Target.Position));
                    }
                }
            }
        }
    }
}