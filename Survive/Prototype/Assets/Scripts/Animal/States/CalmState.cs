using UnityEngine;

namespace Animal.States
{
    public class CalmState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 10f;
        private float maxIntervalTime = 20f;

        public CalmState(AnimalData data) : base(data)
        {
        }

        // when entering state choose the behaviour 
        public override void EnterState(AnimalBase animal)
        {
            ScheduleNextAction();
            if (!data.IsSpawned) return;
            Animal = animal;
            UpdateState();
        }

        public override void UpdateState()
        {
            if (!data.IsSpawned || data.isZoneTraveling) return;

            if (Time.time >= nextActionTime)
            {
                MoveToNextPosition();
            }
        }

        public override void ExitState()
        {
        }

        private void ScheduleNextAction()
        {
            nextActionTime = Time.time + Random.Range(minIntervalTime, maxIntervalTime);
        }

        private void MoveToNextPosition()
        { 
            // Add calm behaviour
            ScheduleNextAction();
        }
    }
}