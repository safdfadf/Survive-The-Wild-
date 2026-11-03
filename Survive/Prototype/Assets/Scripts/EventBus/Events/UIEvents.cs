using System;

namespace SurviveTheWild.EventBus.Events
{
    public class UIEvents
    {
        public Action onRemoveMenu;

        public void RemoveMenu()
        {
            onRemoveMenu?.Invoke();
        }
    }
}