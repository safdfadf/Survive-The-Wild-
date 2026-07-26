using UnityEngine;

namespace Animal.States
{
    public class AlarmState:AnimalState
    {
        public AlarmState(AnimalData animalData) : base(animalData)
        {
            
        }

        public override void EnterState(AnimalBase animal)
        {
            Animal = animal;
           RunOutOfActiveChunk();
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