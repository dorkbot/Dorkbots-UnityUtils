using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    /*
     * TODO: next goal is to create a SteeringBehaviorManager. This is a class that can be extended. It wraps the SteeringBehavior components allowing for switching on and off components and logic. Components will have to reference this manager to get the active SteerBehavior logic.
     * The Manager would have to be a child of the TargetBehavior (or similar) so it can be passed in Serialize Fields. This manager would invoke the action when the steering behavior logic is instantiated and ready
     *
     * 
     */
    public abstract class SteeringBehaviorLogic
    {
        public float MaxSpeed = 3f;
        public float AngularSpeed = 180f;

        public Vector3 DesiredDir = Vector3.zero;
        public float DesiredSpeed = 0;
        
        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }

        public virtual void Init()
        {
            DesiredDir = CalculateDirection();
            DesiredSpeed = CalculateSpeed();
        }
        
        public virtual void Update()
        {
            DesiredDir = CalculateDirection();
            DesiredSpeed = CalculateSpeed();

            Rotate();
            Move();
        }
        
        public void UpdatePositionAndRotation(Vector3 position, Quaternion rotation)
        {
            UpdatePosition(position);
            UpdateRotation(rotation);
        }

        public virtual void UpdatePosition(Vector3 position)
        {
            Position = position;
        }

        public void UpdateRotation(Quaternion rotation)
        {
            Rotation = rotation;
        }

        protected virtual void Move()
        {
            UpdatePosition(Position + (GetForward() * GetCurrentSpeed() * Time.deltaTime));
        }

        protected virtual void Rotate()
        {
            Rotation = Quaternion.RotateTowards(Rotation, Quaternion.LookRotation(DesiredDir), AngularSpeed * Time.deltaTime);
        }
        
        public float GetCurrentSpeed()
        {
            return DesiredSpeed * MaxSpeed;
        }

        protected abstract float CalculateSpeed();
        protected abstract Vector3 CalculateDirection();
        
        protected Vector3 GetForward()
        {
            return Rotation * Vector3.forward;
        }

        protected Vector3 GetUp()
        {
            return Rotation * Vector3.up;
        }

        protected Vector3 GetRight()
        {
            return Rotation * Vector3.right;
        }
    }
}