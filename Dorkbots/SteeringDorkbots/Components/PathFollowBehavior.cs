using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class PathFollowBehavior : SeekAndFleeBehavior
    {
        [Header("PathFollow")] 
        [SerializeField] private bool showPathFollowGizmos;

        [SerializeField] private bool loop = false;
        [SerializeField] private float distanceToChangeTarget = 0.3f;

        private PathFollowBehaviorLogic _pathFollowBehaviorLogic;

        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _pathFollowBehaviorLogic.Loop = loop;
            _pathFollowBehaviorLogic.DistanceToChangeTarget = distanceToChangeTarget;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new PathFollowBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _pathFollowBehaviorLogic = (PathFollowBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showPathFollowGizmos)
            {
                Gizmos.color = Color.magenta;
                if (targets.Count > 0)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (i < targets.Count - 1)
                        {
                            Gizmos.DrawRay(targets[i].transform.position,
                                (targets[i + 1].transform.position - targets[i].transform.position).normalized *
                                Vector3.Distance(targets[i + 1].transform.position, targets[i].transform.position));
                        }
                        else
                        {
                            Gizmos.DrawRay(targets[i].transform.position,
                                (targets[0].transform.position - targets[i].transform.position).normalized *
                                Vector3.Distance(targets[0].transform.position, targets[i].transform.position));
                        }
                    }
                }
            }
        }
    }
}