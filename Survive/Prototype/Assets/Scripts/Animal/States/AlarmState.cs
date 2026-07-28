using UnityEngine;

namespace Animal.States
{
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
            Debug.Log("enterState");
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
            // and  this function will control it , keep approaching the player and change behavior once attack is delivered 
        }

        public override void ExitState()
        {
        }

        private void RunOutOfActiveChunk()
        {
            Vector3 pos =data.GetOutofActiveChunkPos();
            Animal.MoveTo(pos,()=> DeActivateAnimal());
        }

        private void AttackPlayer()
        {
            Animal.Attack();
        }
        private void DeActivateAnimal()
        {
            if(data!=null)
                data.AnimalHandler.DeactivateAnimal(data);
            
            // here how do we deactivate animal we dont have data 
        }
    }
}