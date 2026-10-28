using System;

namespace DefaultNamespace.EventBus.Events
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