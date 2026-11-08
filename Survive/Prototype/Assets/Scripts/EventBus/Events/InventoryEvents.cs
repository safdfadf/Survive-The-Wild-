using System;
using UnityEngine;

namespace SurviveTheWild.EventBus.Events
{
    public class InventoryEvents
    {
        public Action<bool> onInventoryToggle;
        public Action<GameObject> onGatherResource;
        public Action onMenuToggle;

        public void GatherResource(GameObject resource)
        {
            onGatherResource(resource);
        }

        public void InventoryToggle(bool value)
        {
            onInventoryToggle(value);
        }

        public void MenuToggle()
        {
            onMenuToggle();
        }
    }
}