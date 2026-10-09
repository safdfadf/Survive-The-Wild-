using System;

namespace DefaultNamespace.EventBus
{
    public class PlayerEvents
    {
        public Action<int> OnAddExperience;
        public Action OnTriggerPlayerSleep;
        public Action OnDropObjFromHand;
        public void AddExperience(int experience)
        {
            OnAddExperience(experience);
        }

        public void TriggerSleep()
        {
            OnTriggerPlayerSleep();
        }
    }
}