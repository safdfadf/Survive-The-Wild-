using System;
using DefaultNamespace.Interface;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem
{
    public class Container : Obj<ObjSo>
    {
        public bool IsContainerFilled { get; private set; }
        public FoodConsumptionData foodConsumptionData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            useMeDescription = "Collect Water";
        }

        public override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
        }

        public override void UseMe()
        {
            if (IsContainerFilled)
            {
                PlayerRepository.instance.ConsumeFood(foodConsumptionData);
                IsContainerFilled = false;
                useMeDescription = "Collect Water";
            }
            else if (PlayerRepository.instance.waterBody != null)
            {
                StoreWater();
            }
            else
            {
                // Send Notification =>  
                UIManager.instance.DisplayNotification(" There is no water source near by");
            }
        }

        private void StoreWater()
        {
            IsContainerFilled = true;
            var obj = PlayerRepository.instance.waterBody;
            var waterBody = obj.GetComponent<WaterBody>();
            foodConsumptionData = waterBody.GetWater();
            useMeDescription = "Drink";
        }
    }
}