using System;

namespace SurviveTheWild.EventBus.Events
{
    public class TimeEvents
    {
        public Action onOneSecondPass;
        public Action<int> onOneHourPass;
        public Action onFiveSecondsPass;

        public void oneSecondPass()
        {
            onOneSecondPass?.Invoke();
        }

        public void FiveSecondsPass()
        {
            onFiveSecondsPass?.Invoke();
        }
    }
}