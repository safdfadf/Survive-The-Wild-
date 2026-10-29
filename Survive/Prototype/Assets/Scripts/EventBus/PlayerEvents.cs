using System;

namespace DefaultNamespace.EventBus
{
    public class PlayerEvents
    {
        public Action<int> OnAddExperience;
        public Action OnTriggerPlayerSleep;
        public Action OnDropObjFromHand;
        public Action<float> OnDrainStamina;

        public void AddExperience(int experience)
        {
            OnAddExperience(experience);
        }

        public void TriggerSleep()
        {
            OnTriggerPlayerSleep();
        }

        
        public void PayAttackCost(int cost)
        {
            OnDrainStamina(cost);
        }
    }
}