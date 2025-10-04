using System;
using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class FlockBehavior : SeekAndFleeBehavior
    {
        [Header("Flocking")] 
        [SerializeField] private bool showFlockGizmos; 
        
        [SerializeField] private float neighborhoodDistance = 6f;
        [SerializeField] private float separationRadius = 2.5f;
        [Range(0.0f, 1.0f)] 
        [SerializeField] private float aligmentFactor = 0.25f;
        [Range(0.0f, 1.0f)] 
        [SerializeField] private float separationFactor = 0.5f;
        [Range(0.0f, 1.0f)] 
        [SerializeField] private float cohesionFactor = 0.25f;

        private FlockBehaviorLogic _flockBehaviorLogic;
        
        protected override void UpdateParams()
        {
            base.UpdateParams();
            
            _flockBehaviorLogic.NeighborhoodDistance = neighborhoodDistance;
            _flockBehaviorLogic.SeparationRadius = separationRadius;
            _flockBehaviorLogic.AligmentFactor = aligmentFactor;
            _flockBehaviorLogic.SeparationFactor = separationFactor;
            _flockBehaviorLogic.CohesionFactor = cohesionFactor;
        }
        
        protected override void InstantiateLogic()
        {
            SteeringBehaviorLogic = new FlockBehaviorLogic();
        }

        protected override void InitLogic()
        {
            _flockBehaviorLogic = (FlockBehaviorLogic)SteeringBehaviorLogic;
            base.InitLogic();
        }

        private void OnEnable()
        {
            if (_flockBehaviorLogic != null) _flockBehaviorLogic.AddToFlock();
        }

        void OnDisable()
        {
            _flockBehaviorLogic.RemoveFromFlock();
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (showFlockGizmos)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, separationRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, neighborhoodDistance);
            }
        }
    }
}