namespace Dorkbots.SteeringDorkbots.SteeringBehavior
{
    public class PathFollowBehaviorLogic : SeekAndFleeBehaviorLogic
    {
        public bool Loop = false;
        /// <summary>
        /// This needs to be farther than SteeringBehaviorLogic.StopDistance
        /// </summary>
        public float DistanceToChangeTarget = 0.3f;
        
        private int _currentTargetIndex = -1;

        public override void Init()
        {
            if (StopDistance > DistanceToChangeTarget)
            {
                StopDistance = DistanceToChangeTarget;
            }
            base.Init();
        }

        protected override void CalculateTargets()
        {
            if (TargetDistance <= DistanceToChangeTarget)
            {
                NextTarget();
                Target = Targets[_currentTargetIndex];
            }
        }

        public void NextTarget()
        {
            if (_currentTargetIndex < Targets.Count - 1)
            {
                _currentTargetIndex++;
            }
            else if (Loop)
            {
                _currentTargetIndex = 0;
            }
        }
    }
}