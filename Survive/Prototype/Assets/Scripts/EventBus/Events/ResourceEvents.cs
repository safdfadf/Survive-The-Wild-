using System;
using UnityEngine;

namespace DefaultNamespace.EventBus.Events
{
    public class ResourceEvents
    {
        public Action<GameObject> onGatherResource;

        public void GatherResource(GameObject resource)
        {
            onGatherResource(resource);
        }
    }
}