using System;
using DefaultNamespace.Interface;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem
{
    public class Container : Obj<ObjSo>
    {
        public bool IsContainerFilled { get; set; }
        public FoodConsumptionData foodConsumptionData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            UseMeDescription = "Collect Water";
        }

        public override void UseMe()
        {
            if (IsContainerFilled)
            {
                PlayerRepository.instance.ConsumeFood(foodConsumptionData);
                IsContainerFilled = false;
                UseMeDescription = "Collect Water"; // update display 
            }
            else if (PlayerRepository.instance.ContactWithWater != null)
            {
                StoreWater();
            }
            else
            {
                print("no near by water source");
                UIManager.instance.DisplayNotification(" There is no water source near by");
            }
        }

        private void StoreWater()
        {
            // splash Audio 
            // load screen 
            print("store water");
            IsContainerFilled = true;
            var obj = PlayerRepository.instance.ContactWithWater;
            var waterBody = obj.GetComponent<WaterBody>();
            foodConsumptionData = waterBody.GetWater();
            UseMeDescription = "Drink";
        }
    }
}