using System;

namespace DefaultNamespace.EventBus.Events
{
    public class UIEvents
    {
        public Action onRemoveMenu;

        public void RemoveMenu()
        {
            onRemoveMenu();
        }
    }
}