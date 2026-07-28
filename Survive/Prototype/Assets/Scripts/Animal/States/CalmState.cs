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
        public CalmState(){}
        public override void EnterState(AnimalBase animal)
        {
            ScheduleNextAction();
            if (!data.IsSpawned) return;
            Animal = animal;
            UpdateState();
        }

        public override void UpdateState()
        {
            if(data != null && data.isZoneTraveling ) return;
            if (Time.time >= nextActionTime)
            {
                if(Random.value > 0.5f)
                    MoveToNextPosition();
                // if random change that  it will move stay at pos  
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