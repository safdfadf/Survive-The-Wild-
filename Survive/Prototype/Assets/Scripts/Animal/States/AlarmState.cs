using UnityEngine;

namespace Animal.States
{
    // depending on the animal it goes to the attacks n no. of times 
    public class AlarmState : AnimalState
    {
        private bool _isAggresive = false;
        private float _attackCoolDown;
        private int AlarmRadius;

        public AlarmState(AnimalData animalData) : base(animalData)
        {
        }

        public AlarmState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            Animal = animal;
            if (data is { isLeader: true })
            {
                // Charge Call 
                HandleAudioCall(AlarmRadius, Animal.AnimalSo.AlarmClip);
                Animal.HerdCall();
            }

            if (Animal.IsAggresive)
            {
                AttackPlayer();
            }
            else
            {
                RunOutOfActiveChunk();
            }
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public void RunOutOfActiveChunk()
        {
            Debug.Log("Run out of active chunk");
            Vector3 pos = data.GetOutofActiveChunkPos();
            Animal.GetOutOfChunk();
        }

        private void AttackPlayer()
        {
            Animal.Attack();
        }
    }
}