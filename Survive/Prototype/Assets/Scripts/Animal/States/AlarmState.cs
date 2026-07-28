using UnityEngine;

namespace Animal.States
{
    public class AlarmState:AnimalState
    {
        private bool _isAggresive = false;
        public AlarmState(AnimalData animalData) : base(animalData)
        {
            
        }
        public AlarmState(){}
        public override void EnterState(AnimalBase animal)
        {
            Animal = animal;
            _isAggresive = Animal.AnimalSo.isAggresive;
            if (_isAggresive)
            {
                // attack Player 
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
            Vector3 pos =data.GetOutofActiveChunkPos();
            Animal.MoveTo(pos,()=> DeActivateAnimal());
        }
        private void DeActivateAnimal()
        {
            data.AnimalHandler.DeactivateAnimal(data);
        }
    }
}