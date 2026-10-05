using System;

namespace DefaultNamespace.EventBus.Events
{
    public class StructureEvents
    {
        public Action OnSubmitResource;

        public void SubmitResource()
        {
           OnSubmitResource(); 
        }
    }
}