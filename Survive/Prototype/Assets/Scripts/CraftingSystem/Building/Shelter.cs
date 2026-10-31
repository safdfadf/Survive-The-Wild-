using SurviveTheWild.EventBus;
using SurviveTheWild.Interface;
using UnityEngine;

namespace SurviveTheWild.CraftingSystem.Building
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
            SceneRoot.Instance.EventManager.playerEvents.TriggerSleep();
        }
    }
}