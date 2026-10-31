using System;

namespace SurviveTheWild.EventBus.Events
{
    public class TimeEvents
    {
        public Action onOneSecondPass;

        public void oneSecondPass()
        {
            onOneSecondPass?.Invoke();
        }
    }
}