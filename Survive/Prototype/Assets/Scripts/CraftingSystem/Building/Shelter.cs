using DefaultNamespace.EventBus;
using DefaultNamespace.Interface;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem.Building
{
    // so interaction system should be changed a little 
    public class Shelter : BaseStructure
    {
        protected override void Awake()
        {
            base.Awake();
            outlineMe = false;
            canBeCollected = false;
            Description = "Sleep";
            canDisplay = false;
            Gm = gameObject;
        }

        public override void ExecuteAction()
        {
            if (!IsAssembled) return;
            EventManager.Instance.playerEvents.TriggerSleep();
        }

        protected override void OnStructureAssembled()
        {
            canDisplay = true;
        }
    }
}