using UnityEngine;

namespace Animal.States
{
    public class AlarmState:AnimalState
    {
        private AnimalBase _animal;
        public AlarmState(AnimalData animalData) : base(animalData)
        {
            
        }

        public override void EnterState()
        {
            _animal = data.AnimalInstance.GetComponent<AnimalBase>();
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
            _animal.MoveTo(pos,()=> DeActivateAnimal());
        }
        private void DeActivateAnimal()
        {
            data.AnimalHandler.DeactivateAnimal(data);
        }
    }
}