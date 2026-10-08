using System;
using UnityEngine;

namespace DefaultNamespace.EventBus.Events
{
    public class ResourceEvents
    {
        public Action<GameObject> onGatherResource;
        public Action onMenuToggle;

        public void GatherResource(GameObject resource)
        {
            onGatherResource(resource);
        }

        public void MenuToggle()
        {
            onMenuToggle();
        }
    }
}