using UnityEngine;

namespace Animal.States
{
    // depending on the animal it goes to the attacks n no. of times 
    public class AlarmState:AnimalState
    {
        private bool _isAggresive = false;
        private float _attackCoolDown;
        public AlarmState(AnimalData animalData) : base(animalData)
        {
            
        }
        public AlarmState(){}
        public override void EnterState(AnimalBase animal)
        {
            // for alert state when running some animals like small game will only run in active zone
            Animal = animal;
            _isAggresive = Animal.AnimalSo.isAggresive;
            if (_isAggresive)
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

        private void RunOutOfActiveChunk()
        {
            Debug.Log("Run out of active chunk");
            Vector3 pos =data.GetOutofActiveChunkPos();
            Animal.MoveTo(pos,()=> DeActivateAnimal());
        }

        private void AttackPlayer()
        {
            Debug.Log("attack" + Animal.AnimalSo.name);
           Animal.Attack();
        }
        private void DeActivateAnimal()
        {
            if(data!=null)
                data.AnimalHandler.DeactivateAnimal(data);
                   
        }
    }
}