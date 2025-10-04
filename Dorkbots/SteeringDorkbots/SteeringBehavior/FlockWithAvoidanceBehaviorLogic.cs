using UnityEngine;

namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class FlockWithAvoidanceBehaviorLogic : FlockBehaviorLogic
    {
        [Range(0, 1)] public float VerticalAngle = 1;
        public int HorizontalAccuracy = 0;
        public LayerMask Mask;
        public float RaycastDistance;
        [Range(0f, 1f)] public float Separation = 1f;
        [Range(1f, 100f)] public float CorrectionFactor = 50f;

        private float _front, _left, _right, _up, _down;
        private Vector3 _frontRaycast;
        private RaycastHit _raycastHitFront, _raycastHitLeft, _raycastHitRight, _raycastHitUp, _raycastHitDown;
        private Vector3 _directionCalculated;
        private bool _obstacles;

        //This is the same math as Avoiding
        protected override Vector3 CalculateDirection()//TODO: cache forward and right and up
        {
            if (Target == null) return Vector3.zero;
            
            _obstacles = true;
            _frontRaycast = GetForward().normalized * RaycastDistance;

            _front = _left = _right = _up = _down = 0;

            Physics.Raycast(Position, _frontRaycast.normalized, out _raycastHitFront, RaycastDistance * 2, Mask);
            _front = _raycastHitFront.distance;
            Physics.Raycast(Position, (GetForward() * VerticalAngle + GetUp() * (1 - VerticalAngle)).normalized, out _raycastHitUp, RaycastDistance, Mask);
            _up = _raycastHitUp.distance;
            Physics.Raycast(Position, (GetForward() * VerticalAngle - GetUp() * (1 - VerticalAngle)).normalized, out _raycastHitDown, RaycastDistance * 2, Mask);
            _down = _raycastHitDown.distance;

            float subdivision = (90f / (float) HorizontalAccuracy) / 100f;
            for (int i = 0; i < HorizontalAccuracy; i++)
            {
                Physics.Raycast(Position, ((GetForward() * (subdivision * i)) + GetRight() * (HorizontalAccuracy - i) * subdivision).normalized, out _raycastHitLeft, RaycastDistance, Mask);
                float rightTemp = _raycastHitLeft.distance;
                Physics.Raycast(Position, ((GetForward() * (subdivision * i)) - GetRight() * (HorizontalAccuracy - i) * subdivision).normalized, out _raycastHitRight, RaycastDistance, Mask);
                float leftTemp = _raycastHitRight.distance;

                if (rightTemp > _right) _right = rightTemp;

                if (leftTemp > _left) _left = leftTemp;
            }

            if (_front != 0 && _right <= _left)
            {
                _directionCalculated = GetRight();
            }
            else if (_front != 0 && _right > _left)
            {
                _directionCalculated = -GetRight();
            }
            else if (_front == 0 && _right < _left)
            {
                if (GetForward().z < Separation)
                    _directionCalculated = GetForward() + GetRight() / CorrectionFactor;
                else
                    _directionCalculated = GetForward();
            }
            else if (_front == 0 && _right > _left)
            {
                if (GetForward().z > -Separation)
                    _directionCalculated = GetForward() - GetRight() / CorrectionFactor;
                else
                    _directionCalculated = GetForward();
            }
            else
            {
                _directionCalculated = Target.Position - Position;
                _obstacles = false;
            }

            if (VerticalAngle != 1)
            {
                if (_up < _down)
                {
                    _directionCalculated += GetUp();
                }
                else if (_up > _down)
                {
                    _directionCalculated += -GetUp();
                }
            }

            if (!_obstacles)
            {
                return base.CalculateDirection();
            }
            else
            {
                CurrentTarget = InitialTarget;
                return _directionCalculated;
            }
        }
    }
}