using System;
using SurviveTheWild.Interface;

namespace SurviveTheWild.EventBus
{
    public class PlayerEvents
    {
        public Action<int> OnAddExperience;
        public Action OnTriggerPlayerSleep;
        public Action OnDropObjFromHand;
        public Action<float> OnDrainStamina;
        public Action<bool> onOutOfBreath;
        public Action<IRequestInput> onDeliverRequestInput;
        public Action onRemoveRequestInput;

        public void AddExperience(int experience)
        {
            OnAddExperience(experience);
        }

        public void PlayerOutOfBreath(bool isOutOfBreath)
        {
            onOutOfBreath?.Invoke(isOutOfBreath);
        }

        public void TriggerSleep()
        {
            OnTriggerPlayerSleep();
        }


        public void PayAttackCost(int cost)
        {
            OnDrainStamina(cost);
        }

        public void DeliverRequestInput(IRequestInput input)
        {
            onDeliverRequestInput?.Invoke(input);
        }

        public void ResetRequestedInput()
        {
            onRemoveRequestInput();
        }
    }
}