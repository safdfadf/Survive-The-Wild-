using System;

namespace SurviveTheWild.EventBus.Events
{
    public class AnimalEvents
    {
        public Action<AnimalBase> onAnimalDeath;
        public Action<LodLevel> onUpdateLodLevel;

        public void AnimalDeath(AnimalBase value)
        {
            onAnimalDeath(value);
        }

        public void UpdateLodLevel(LodLevel value)
        {
            onUpdateLodLevel(value);
        }
    }
}