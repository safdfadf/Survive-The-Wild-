using UnityEngine;

namespace Animal.States
{
    public class AlarmState : AnimalState
    {
        private float _attackCoolDown;

        private int AlarmRadius = 15;

        // what should be the radius for alarm state 
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
                HandleAudioCall(AlarmRadius,
                    Animal.AnimalSo.AlarmClip[Random.Range(0, Animal.AnimalSo.AlarmClip.Count)]);
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
            Vector3 pos = data.GetOutofActiveChunkPos();
            Animal.GetOutOfChunk();
        }

        private void AttackPlayer()
        {
            Animal.Attack();
        }
    }
}