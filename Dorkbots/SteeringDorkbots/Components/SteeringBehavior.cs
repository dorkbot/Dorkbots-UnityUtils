using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public abstract class SteeringBehavior : TargetBehavior
    {
        [Header("Steering Behaviour Basic")] 
        [Tooltip("True will update the logic's parameters every frame. Recommend only using for testing...")]
        [SerializeField] protected bool updateLogicParams = false;
        [SerializeField] private bool showSteeringGizmos;

        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float angularSpeed = 180f;

        private Vector3 _desiredDir;
        private float _desiredSpeed;

        protected override void Update()
        {
            if (updateLogicParams && SteeringBehaviorLogic != null) UpdateParams();
            base.Update();
        }

        protected virtual void UpdateParams()
        {
            SteeringBehaviorLogic.MaxSpeed = maxSpeed;
            SteeringBehaviorLogic.AngularSpeed = angularSpeed;
        }
        
        protected override void InstantiateLogic()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitLogic()
        {
            UpdateParams();
            SteeringBehaviorLogic.Init();
            base.InitLogic();
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            if (showSteeringGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + _desiredDir.normalized * 2);
            }
        }
    }
}