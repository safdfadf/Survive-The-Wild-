using System;
using UnityEngine;

namespace SurviveTheWild.EventBus.Events
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