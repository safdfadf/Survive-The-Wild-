using UnityEngine;

namespace Animal.States
{
    public class AlertState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 5f;
        private float maxIntervalTime = 20f;
        private int AlarmRadius;

        public AlertState(AnimalData data) : base(data)
        {
        }

        public AlertState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            //  ScheduleNextAction();
            Animal = animal;
            AnimalAlert();
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        private void ScheduleNextAction()
        {
            nextActionTime = Time.time + Random.Range(minIntervalTime, maxIntervalTime);
        }

        private void AnimalAlert()
        {
            if (Animal == null) return;
            Animal.TriggerAlertAnim();
            // warning Call 
            HandleAudioCall(AlarmRadius, Animal.AnimalSo.warningClip);
        }
    }
}