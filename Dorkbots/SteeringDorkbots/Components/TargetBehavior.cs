using System;
using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    public class TargetBehavior : MonoBehaviour
    {
        public enum PositionHandlingTypes
        {
            BehaviorLogicUpdatesPosition,
            ThisBehaviorUpdatesPosition,
            PositioningIgnoredPeformUpdate,
            PositioningIgnoredDontUpdate
        }

        [SerializeField] protected PositionHandlingTypes positionHandlingType = PositionHandlingTypes.BehaviorLogicUpdatesPosition;

        public bool Armed { get; private set; } = false;
        public event Action<SteeringBehaviorLogic> LogicInstantiatedAction;
        public event Action<TargetBehavior> TargetArmedAction;
        public event Action<TargetBehavior> TargetDisarmedAction;
        
        public SteeringBehaviorLogic SteeringBehaviorLogic { get; protected set; }
        
        protected virtual void Awake()
        {
            InstantiateLogic();
            InitLogic();
        }
        
        protected virtual void Update()
        {
            if (SteeringBehaviorLogic != null)
            {
                switch (positionHandlingType)
                {
                    case PositionHandlingTypes.BehaviorLogicUpdatesPosition:
                        SteeringBehaviorLogic.Update();
                        transform.position = SteeringBehaviorLogic.Position;
                        transform.rotation = SteeringBehaviorLogic.Rotation;
                        break;
                    
                    case PositionHandlingTypes.ThisBehaviorUpdatesPosition:
                        SteeringBehaviorLogic.UpdatePositionAndRotation(transform.position, transform.rotation);
                        SteeringBehaviorLogic.Update();
                        break;
                    
                    case PositionHandlingTypes.PositioningIgnoredPeformUpdate:
                        SteeringBehaviorLogic.Update();
                        break;
                }
            }
        }

        private void OnEnable()
        {
            Armed = true;
            TargetArmedAction?.Invoke(this);
        }

        private void OnDisable()
        {
            Armed = false;
            TargetDisarmedAction?.Invoke(this);
        }

        protected virtual void InstantiateLogic()
        {
            SteeringBehaviorLogic = new TargetBehaviorLogic();
        }

        protected virtual void InitLogic()
        {
            SteeringBehaviorLogic.UpdatePositionAndRotation(transform.position, transform.rotation);
            LogicInstantiatedAction?.Invoke(SteeringBehaviorLogic);
        }

        public void SetPositionHandlingTypes(PositionHandlingTypes type)
        {
            positionHandlingType = type;
        }
    }
}