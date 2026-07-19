using UnityEngine;

namespace Animal.States
{
    public class CalmState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 10f;
        private float maxIntervalTime = 20f;
        private AnimalBase animal;

        // logic for calm state: when player is in calm state it moves around the zone and do feeding animation 
        public CalmState(AnimalData data) : base(data)
        {
        }

        public override void EnterState()
        {
            // Debug.Log("Enter Calm State");
            ScheduleNextAction();
            if (!data.IsSpawned) return;
            // Start idle feeding/drinking animation
            animal = data.AnimalInstance.GetComponent<AnimalBase>();
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
            Vector3 origin = animal.transform.position;
            Vector3 offset = Random.insideUnitSphere * .5f;
            Vector3 destination = origin + new Vector3(offset.x, 0, offset.z);
            Vector3 oldPos = animal.currentPos.Value;
            animal.currentZone.ReleasePosition(oldPos);
            Vector3? newPos = animal.currentZone.RequestPosition();
            if (newPos == null) return;
            //animal.MoveTo(newPos.Value);
            ScheduleNextAction();
        }
    }
}