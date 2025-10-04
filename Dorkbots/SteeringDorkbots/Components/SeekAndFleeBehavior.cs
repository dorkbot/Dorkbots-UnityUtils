using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class SeekAndFleeBehavior : SteeringBehaviorWithTargets
    {
        [Header("SeekAndFlee")]
        [SerializeField] private bool showSeekAndFleeGizmos;

        [SerializeField] private bool flee = false;
        [SerializeField] private float brakingDistance = 2f;

        private SeekAndFleeBehaviorLogic _seekAndFleeBehaviorLogic;

        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _seekAndFleeBehaviorLogic.Flee = flee;
            _seekAndFleeBehaviorLogic.BrakingDistance = brakingDistance;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new SeekAndFleeBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _seekAndFleeBehaviorLogic = (SeekAndFleeBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showSeekAndFleeGizmos)
            {
                Gizmos.color = Color.yellow;

                if (!multipleTargets && target != null)
                {
                    Gizmos.DrawWireSphere(target.transform.position, brakingDistance);
                }
                else if (multipleTargets && targets.Count > 0)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        Gizmos.DrawWireSphere(targets[i].transform.position, brakingDistance);
                    }
                }
            }
        }
    }
}