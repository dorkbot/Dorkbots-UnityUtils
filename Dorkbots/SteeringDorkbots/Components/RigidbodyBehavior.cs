using System;
using Dorkbots.MonoBehaviorTools;
using Dorkbots.SteeringDorkbots.SteeringBehavior;
using UnityEngine;

namespace Dorkbots.SteeringDorkbots.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyBehavior : MonoBehaviour
    {
        [SerializeField] private bool showGizmos;
        [SerializeField] private SteeringBehavior steeringBehavior;
        
        [Header("Collision Behavior")] 
        [SerializeField] private float collisionReflectStunTime = .1f;//recommend .2 for useVelocityToMove
        [SerializeField] private float collisionBehaviorTime = 1;
        [SerializeField] private float collisionReflectionAmount = .5f;
        
        [Header("Collision Stun Behavior for an Avoid Obstacle Behavior")] 
        [SerializeField] private float collisionStunRaycastDistance = 10;
        [SerializeField] private float collisionStunCorrectionFactor = 100;
        [SerializeField] private float collisionStunMaxSpeedMultiplier = .5f;
        [SerializeField] private float collisionAngularSpeed = 360;
        [SerializeField] private float collisionLerpTimeRotationToReflect = .5f;
        
        [Header("Too Many Collisions")] 
        [SerializeField] private float tooManyCollisionsTime = .8f;
        [SerializeField] private float tooManyCollisionsReflectStunTime = .7f;//recommend .2 for useVelocityToMove
        [SerializeField] private float tooManyCollisionsBehaviorTime = 2;
        [SerializeField] private float tooManyCollisionsReflectionAmount = 1;

        //It appears that when the max speed is greater than the avoid raycast, then the steering behavior can end up inside a collider. And that will cause issues when using velocity to move
        [Header("Use Velocity Instead of MovePosition and MoveRotation")]
        [SerializeField] private bool useVelocityToMove = false;

        private bool _collisionStunned = false;
        private float _collisionStunnedStartTime;
        public bool CollisionBehavior { get; private set; } = false;
        
        private SimpleTimerCoroutine _collisionStunCoroutine;
        private SimpleTimerCoroutine _collisionBehaviorCoroutine;
        private FlockWithAvoidanceBehaviorLogic _avoidObstacleBehaviorLogic;
        private float _beforeCollisionStunRaycastDistance;
        private float _beforeCollisionStunCorrectionFactor;
        private float _beforeCollisionStunMaxSpeed;
        private float _beforeCollisionAngularSpeed;
        
        private Rigidbody _rigidbody;
        private SteeringBehaviorLogic _steeringBehaviorLogic;
        private Vector3 _movePosition = Vector3.zero;

        private float _timeOfLastCollision;
        private int _collisionsWithinTimeCounter = 0;
        private Quaternion _reflectedDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (steeringBehavior == null) steeringBehavior = GetComponent<SteeringBehavior>();
        }
        
        void Start()
        {
            _collisionStunnedStartTime = _timeOfLastCollision = Time.time;
            _movePosition = transform.position;
            
            steeringBehavior.SetPositionHandlingTypes(TargetBehavior.PositionHandlingTypes.PositioningIgnoredDontUpdate);
            if (steeringBehavior.SteeringBehaviorLogic != null)
            {
                SetupSteeringBehaviorLogic(steeringBehavior.SteeringBehaviorLogic);
            }
            else
            {
                steeringBehavior.LogicInstantiatedAction += SetupSteeringBehaviorLogic;
            }
            
            CollisionStunCoroutineFactory(collisionReflectStunTime);
            CollisionBehaviorCoroutineFactory(collisionBehaviorTime);
        }

        private void Update()
        {
            if (_steeringBehaviorLogic == null) return;
            
            if (_collisionStunned)
            {
                _steeringBehaviorLogic.UpdatePositionAndRotation(transform.position, transform.rotation);//ignore steering behavior
                //if (_collisionsWithinTimeCounter > 1) _steeringBehaviorLogic.Update();
            }
            else
            {
                _steeringBehaviorLogic.Update(); 
            }
            
            _movePosition = _steeringBehaviorLogic.Position;
        }

        void FixedUpdate()
        {
            if (_steeringBehaviorLogic == null || _collisionStunned)
            {
                float lerpAmount;
                float timeLapsed = Time.time - _collisionStunnedStartTime;
                if (timeLapsed < collisionLerpTimeRotationToReflect)
                {
                    lerpAmount = timeLapsed / collisionLerpTimeRotationToReflect;
                }
                else
                {
                    lerpAmount = 1;
                }
                
                _rigidbody.MoveRotation(Quaternion.Lerp( _rigidbody.rotation, _reflectedDirection, lerpAmount));//rotate in direction of reflection
                
                //if (_collisionsWithinTimeCounter > 1) _rigidbody.MoveRotation(_steeringBehaviorLogic.Rotation);
                //if (_collisionsWithinTimeCounter > 1)
                //{
                    // Calculate the Lerp factor (0 to 1) based on elapsedTime and rotationTimemand Limit the lerpFactor to be between 0 and 1
                    //_rigidbody.MoveRotation(Quaternion.Lerp( _rigidbody.rotation, _reflectedDirection, Mathf.Clamp01((Time.time - _collisionStunnedStartTime) / _collisionReflectStunTimeCurrent)));//rotate in direction of reflection
                //}
                
                return;
            }
            
            
            _rigidbody.MoveRotation(_steeringBehaviorLogic.Rotation);
            
            if (!useVelocityToMove)
            {
                _rigidbody.MovePosition(_movePosition);
            }
            else
            {
                Vector3 velocity = (_steeringBehaviorLogic.Position - _rigidbody.transform.position) / Time.fixedDeltaTime;
               
                // Clamp the velocity to a maximum speed
                //velocity = Vector3.ClampMagnitude(velocity, _steeringBehaviorLogic.MaxSpeed);
               
                // Apply the velocity to the Rigidbody
                _rigidbody.velocity = velocity;
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            CreateCollisionReflection(collision, collisionReflectionAmount);
        }

        private void OnCollisionStay(Collision collision)
        {
            CreateCollisionReflection(collision, collisionReflectionAmount);
        }
        
        private void CreateCollisionReflection(Collision collision, float reflectAmount)
        {
            if (Time.time - _timeOfLastCollision > tooManyCollisionsTime) _collisionsWithinTimeCounter = 0;//not tested with useVelocityToMove
            _timeOfLastCollision = Time.time;
            _collisionsWithinTimeCounter++;//not tested with useVelocityToMove
            if (_collisionsWithinTimeCounter > 1)//not tested with useVelocityToMove
            {
                CollisionStunCoroutineFactory(tooManyCollisionsReflectStunTime);//not tested with useVelocityToMove
                CollisionBehaviorCoroutineFactory(tooManyCollisionsBehaviorTime);//not tested with useVelocityToMove
                reflectAmount = Math.Min(tooManyCollisionsReflectionAmount, collision.relativeVelocity.magnitude);
            }
            else
            {
                reflectAmount = Math.Min(reflectAmount, collision.relativeVelocity.magnitude);
            }

            if (reflectAmount <= 0) reflectAmount = .01f;//we don't want a zero vector
            
            if (useVelocityToMove)
            {
                PerformCollision(Vector3.Reflect(_rigidbody.velocity.normalized, collision.contacts[0].normal).normalized * reflectAmount);//maybe don't need to normalize the reflect?
            }
            else
            {
                PerformCollision(Vector3.Reflect(transform.forward, collision.contacts[0].normal).normalized * reflectAmount);
            }
            
            if (_collisionsWithinTimeCounter > 1)//not tested with useVelocityToMove
            {
                CollisionStunCoroutineFactory(collisionReflectStunTime);//not tested with useVelocityToMove
                CollisionBehaviorCoroutineFactory(collisionBehaviorTime);//not tested with useVelocityToMove //TODO: not the best solution, now can't stop coroutine
            }
        }
        
        public void PerformCollision(Vector3 reflectedDirection)//TODO: maybe we need to toggle bool for lerping rotation during stun, maybe we only want to lerp rotation when colliding with objects and not getting attacked as an example
        {
            if (_steeringBehaviorLogic == null) return;
            if (!_collisionStunned && useVelocityToMove) _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(reflectedDirection, ForceMode.Impulse);
            
            _steeringBehaviorLogic.UpdatePositionAndRotation(transform.position, transform.rotation);

            if (!_collisionStunned) _reflectedDirection = Quaternion.LookRotation(reflectedDirection);
            
            if (!enabled) return;
            _collisionStunned = true;

            _collisionStunnedStartTime = Time.time;
            _collisionStunCoroutine.Start();
            
            if (_avoidObstacleBehaviorLogic != null && !CollisionBehavior)
            {
                _beforeCollisionStunRaycastDistance = _avoidObstacleBehaviorLogic.RaycastDistance;
                _beforeCollisionStunCorrectionFactor = _avoidObstacleBehaviorLogic.CorrectionFactor;
                _beforeCollisionStunMaxSpeed = _steeringBehaviorLogic.MaxSpeed;
                _beforeCollisionAngularSpeed = _steeringBehaviorLogic.AngularSpeed;
                
                _avoidObstacleBehaviorLogic.RaycastDistance = collisionStunRaycastDistance;
                _avoidObstacleBehaviorLogic.CorrectionFactor = collisionStunCorrectionFactor;
                _steeringBehaviorLogic.MaxSpeed *= collisionStunMaxSpeedMultiplier;
                _steeringBehaviorLogic.AngularSpeed = collisionAngularSpeed;
            }

            CollisionBehavior = true;
            _collisionBehaviorCoroutine.Start();
        }

        protected void OnDrawGizmosSelected()
        {
            if (showGizmos && _steeringBehaviorLogic != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(_steeringBehaviorLogic.Position, .5f);
                
                Gizmos.color = Color.black;
                Gizmos.DrawLine(transform.position, _movePosition);
            }
        }

        private void SetupSteeringBehaviorLogic(SteeringBehaviorLogic behaviorLogic)
        {
            _steeringBehaviorLogic = behaviorLogic;
            if (_steeringBehaviorLogic is FlockWithAvoidanceBehaviorLogic logic)
            {
                _avoidObstacleBehaviorLogic = logic;
            }
        }

        private void CollisionStunCoroutineFactory(float time)
        {
            _collisionStunCoroutine = StartStopCoroutine.CreateSimpleTimerCoroutine(time, this, () =>
            {
                _steeringBehaviorLogic.UpdatePositionAndRotation(transform.position, transform.rotation);
                _collisionStunned = false;
            });
        }

        private void CollisionBehaviorCoroutineFactory(float time)
        {
            _collisionBehaviorCoroutine = StartStopCoroutine.CreateSimpleTimerCoroutine(time, this, () =>
            {
                if (_avoidObstacleBehaviorLogic != null)
                {
                    _avoidObstacleBehaviorLogic.RaycastDistance = _beforeCollisionStunRaycastDistance;
                    _avoidObstacleBehaviorLogic.CorrectionFactor = _beforeCollisionStunCorrectionFactor;
                    _steeringBehaviorLogic.MaxSpeed = _beforeCollisionStunMaxSpeed;
                    _steeringBehaviorLogic.AngularSpeed = _beforeCollisionAngularSpeed;
                }

                CollisionBehavior = false;
            });
        }
    }
}