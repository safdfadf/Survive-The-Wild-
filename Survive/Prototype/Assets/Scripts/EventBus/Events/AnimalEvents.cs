using System;

namespace SurviveTheWild.EventBus.Events
{
    public class AnimalEvents
    {
        public Action<AnimalBase> onAnimalDeath;

        public void AnimalDeath(AnimalBase value)
        {
            onAnimalDeath(value);
        }
    }
}