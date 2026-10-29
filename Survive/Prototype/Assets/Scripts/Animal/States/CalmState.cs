using Player;
using UnityEngine;

namespace Animal.States
{
    public class CalmState : AnimalState
    {
        private float nextActionTime;
        private float minIntervalTime = 10f;
        private float maxIntervalTime = 20f;
        private int CalmRadius;

        public CalmState(AnimalData data) : base(data)
        {
        }

        public CalmState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            ScheduleNextAction();
            Animal = animal;
            UpdateState();
        }

        public override void UpdateState() // for now updating every 5 secs
        {
            if (data != null && data.isZoneTraveling ||
                !ChunkManager.Instance.IsPosInPlayerChunk(Animal.transform.position) ||
                !Animal.AnimalSo.inZoneTravel) return;
            if (Random.value > 0.5f)
                MoveToNextPosition();

            if (Random.value > 0.5f)
                HandleAudioCall(CalmRadius, Animal.AnimalSo.calmClip);
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
            Chunk chunk = ChunkManager.Instance.GetChunkAtPos(Animal.transform.position);
            Vector3 pos = RetPosOnNv.ReturnRandomNavMeshPos(chunk.bounds);
            Animal.MoveTo(pos);
            ScheduleNextAction();
        }

        protected override void HandleAudioCall(int Radius, AudioClip Clip)
        {
            if (!PlayerRepository.instance.IsPlayerInRadius(Radius))
                return; // and if player is in Calm Radius 
            base.HandleAudioCall(Radius, Clip);
        }
    }
}