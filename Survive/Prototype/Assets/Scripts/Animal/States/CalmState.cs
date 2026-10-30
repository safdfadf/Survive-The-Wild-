using Player;
using UnityEngine;

namespace Animal.States
{
    public class CalmState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 10f;
        private float maxIntervalTime = 20f;
        private int CalmRadius = 100;
        private bool canUpdate = false;

        public CalmState(AnimalData data) : base(data)
        {
        }

        public CalmState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            canUpdate = true;
            ScheduleNextAction();
            Animal = animal;
            UpdateState();
        }

        public override void UpdateState() // for now updating every 5 secs
        {
            if (!canUpdate) return;
            if (data != null && data.isZoneTraveling || Animal == null)
            {
                Debug.Log(data.isZoneTraveling);
                return;
            }

            if (!(Time.time >= nextActionTime))
            {
                return;
            }

            if (Random.value > 0.5f && Animal.AnimalSo.inZoneTravel)
                MoveToNextPosition();

            if (Random.value > 0.5f)
                Debug.Log("updating Calm State ");
            HandleAudioCall(CalmRadius, Animal.AnimalSo.calmClip[Random.Range(0, Animal.AnimalSo.calmClip.Count)]);
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
            Debug.Log("in zone travel");
            Chunk chunk = ChunkManager.Instance.GetChunkAtPos(Animal.transform.position);
            Vector3 pos = RetPosOnNv.ReturnRandomNavMeshPos(chunk.bounds);
            Animal.MoveTo(pos);
            ScheduleNextAction();
        }
    }
}