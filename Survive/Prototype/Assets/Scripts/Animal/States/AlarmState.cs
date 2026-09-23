using UnityEngine;

namespace Animal.States
{
    // depending on the animal it goes to the attacks n no. of times 
    public class AlarmState : AnimalState
    {
        private bool _isAggresive = false;
        private float _attackCoolDown;

        public AlarmState(AnimalData animalData) : base(animalData)
        {
        }

        public AlarmState()
        {
        }

        public override void EnterState(AnimalBase animal)
        {
            Animal = animal;
            _isAggresive = Animal.AnimalSo.isAggresive;
            if (data is { isLeader: true }) // every animal in the zone is calling and changing states 
            {
                Animal.HerdCall();
            }

            if (_isAggresive)
            {
                AttackPlayer();
            }
            else
            {
                RunOutOfActiveChunk();
            }
        }

        private void HerdCall(AnimalBase animal)
        {
            if (data.AnimalHandler.Attacker == null && _isAggresive)
            {
                data.AnimalHandler.Attacker = animal;
            }

            data.AnimalHandler.HerdWarning(animal, data.GetCurrentZone());
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