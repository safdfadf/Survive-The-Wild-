using System;

namespace DefaultNamespace.EventBus.Events
{
    public class CraftEvents
    {
        public Action<ObjSo> OnCraft;

        public void ObjectCraft(ObjSo so)
        {
            OnCraft?.Invoke(so);
        }
    }
}