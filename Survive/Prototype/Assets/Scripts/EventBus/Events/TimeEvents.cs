using System;

namespace SurviveTheWild.EventBus.Events
{
    public class TimeEvents
    {
        public Action onOneSecondPass;
        public Action<int> onOneHourPass;
        public void oneSecondPass()
        {
            onOneSecondPass?.Invoke();
        }
    }
}