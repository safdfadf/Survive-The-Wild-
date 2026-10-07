using DefaultNamespace.EventBus;
using DefaultNamespace.Interface;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem.Building
{
    //how will this work with the new interaction system  
    public class Shelter : BaseStructure
    {
        protected override void Awake()
        {
            base.Awake();
            Description = "Sleep";
        }

        public override void ExecuteAction()
        {
            if (!IsAssembled) return;
            EventManager.Instance.playerEvents.TriggerSleep();
        }
    }
}