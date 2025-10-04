using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class PursuitAndEvade : SteeringBehaviorWithTargets
    {
        [Header("PursuitAndEvadeLogic")] 
        [SerializeField] private bool showPursuitAndEvadeGizmos;
        [SerializeField] private bool evade = false;
        [SerializeField] private float brakingDistance = 3f;

        private PursuitAndEvadeLogic _pursuitAndEvadeLogic;
        
        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _pursuitAndEvadeLogic.Evade = evade;
            _pursuitAndEvadeLogic.BrakingDistance = brakingDistance;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new PursuitAndEvadeLogic();
        }

        protected override void InitLogic()
        {
            _pursuitAndEvadeLogic = (PursuitAndEvadeLogic) SteeringBehaviorLogic;
            base.InitLogic();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showPursuitAndEvadeGizmos)
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

                if (_pursuitAndEvadeLogic != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(_pursuitAndEvadeLogic.FutureTargetPosition, 0.2f);

                    Gizmos.color = Color.magenta;
                    Gizmos.DrawRay(transform.position,
                        Vector3.Distance(transform.position, _pursuitAndEvadeLogic.FutureTargetPosition) *
                        (_pursuitAndEvadeLogic.FutureTargetPosition - transform.position).normalized);
                }
            }
        }
    }
}