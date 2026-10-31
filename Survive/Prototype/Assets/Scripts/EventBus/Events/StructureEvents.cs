using System;

namespace SurviveTheWild.EventBus.Events
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