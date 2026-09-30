using DefaultNamespace.EventBus;
using DefaultNamespace.Interface;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem.Building
{
    // so interaction system should be changed a little 
    public class Shelter:BaseStructure,IInteractable,IInteractionUI
    { 
        public bool canDisplay { get; set; }
        public string useMeDescription { get; set; }
        public string Description { get; set; }
        public bool canUse { get; set; }
        public bool canHarvest { get; set; }
        public bool canCraft { get; set; }
        public bool outlineMe { get; set; }
        public bool canBeCollected { get; set; }
        public GameObject Gm { get; set; }
        public bool isHit { get; set; }
        public Vector3 hitPos { get; set; }
        protected override void Awake()
        {
            outlineMe = false;
            canBeCollected = false;
            Description = "Sleep";
            canDisplay = true;
            base.Awake();
        }

        public void ExecuteAction()
        {
            
            EventManager.Instance.playerEvents.TriggerSleep();
        }

       
        public void Craft()
        {
            throw new System.NotImplementedException();
        }

        public void Harvest()
        {
            throw new System.NotImplementedException();
        }

        public void UseMe()
        {
            throw new System.NotImplementedException();
        }

        public GameObject obj { get; set; }
    }
}