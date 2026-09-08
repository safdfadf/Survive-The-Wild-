using System;

namespace DefaultNamespace.EventBus
{
    public class PlayerEvents
    {
        public Action<int> OnAddExperience;

        public void AddExperience(int experience)
        {
            OnAddExperience(experience);
        }
    }
}