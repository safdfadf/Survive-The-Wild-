using System;
using DefaultNamespace.Interface;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace.CraftingSystem
{
    public class Container : Obj<ObjSo>
    {
        private bool isContainerFilled;
        private FoodConsumptionData foodConsumptionData;

        protected override void Awake()
        {
            base.Awake();
            useMeDescription = "Drink";
        }

        protected override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
        }

        public override void UseMe()
        {
            if (isContainerFilled)
            {
                PlayerRepository.instance.ConsumeFood(foodConsumptionData);
                isContainerFilled = false;
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
            isContainerFilled = true;
            var obj = PlayerRepository.instance.waterBody;
            var waterBody = obj.GetComponent<WaterBody>();
            foodConsumptionData = waterBody.GetWater();
        }
    }
}