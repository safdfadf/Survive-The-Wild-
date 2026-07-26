using UnityEngine;

namespace Animal.States
{
    public class AlertState:AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 5f;
        private float maxIntervalTime = 20f;
        public AlertState(AnimalData data) : base(data)
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            ScheduleNextAction();
            if (!data.IsSpawned)return;
            // Start idle feeding/drinking animation
            Animal =animal;
        }

        public override void UpdateState()
        {
            if(!data.IsSpawned || data.isZoneTraveling)return;
            // we need agents reference we do the math and call move function 
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
            
            // in the radius of .5f move to any pos 
            Vector3 origin = Animal.transform.position;
            Vector3  offset = Random.insideUnitSphere * .5f;
            Vector3 destination = origin + new Vector3(offset.x, 0, offset.z);
            Animal.MoveTo(destination);
             
        }

    }
}