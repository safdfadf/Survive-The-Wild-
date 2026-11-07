using System;

namespace SurviveTheWild.EventBus.Events
{
    public class UIEvents
    {
        public Action onRemoveMenu;
        public Action<bool> onToggleInventory;

        public void RemoveMenu()
        {
            onRemoveMenu?.Invoke();
        }

        public void ToggleInventory(bool value)
        {
            onToggleInventory(value);
        }
    }
}