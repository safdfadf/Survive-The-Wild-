using UnityEngine;

namespace Animal.States
{
    public class AlertState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 5f;
        private float maxIntervalTime = 20f;

        public AlertState(AnimalData data) : base(data)
        {
        }

        public AlertState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            ScheduleNextAction();
            Animal = animal;
            AnimalAlert();
        }

        public override void UpdateState()
        {
            if (Time.time >= nextActionTime)
            {
                AnimalAlert();
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
            Vector3 offset = Random.insideUnitSphere * .5f;
            Vector3 destination = origin + new Vector3(offset.x, 0, offset.z);
            Animal.MoveTo(destination);
        }

        private void AnimalAlert()
        {
            if (Animal == null) return;
            Animal.TriggerAlertAnim();
        }
    }
}