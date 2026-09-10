using Player;
using UnityEngine;

namespace FoodSystem
{
    public class WaterObj : Obj<ObjSo>
    {
        public WaterState waterState { get; private set; }
        [SerializeField] private FoodSo waterSo;
        [SerializeField] private float minPoisonAmount;
        [SerializeField] private float maxPoisonAmount;
        private FoodConsumptionData data = new();

        protected override void Awake()
        {
            base.Awake();
            canBeCollected = false;
            rb = null;
            Gm = gameObject;
            So = waterSo;
            data.NutrientsCount = waterSo.nutrientsCount;
        }

        public void SetWaterObject(WaterState state)
        {
            waterState = state;
        }

        public override void UseMe()
        {
            // trigger drink animation 
            // trigger audio 

            PlayerRepository.instance.ConsumeFood(data);
        }


        protected override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
        }
    }
}

public enum WaterState
{
    Safe,
    UnSafe,
    Dirty
}

public class WaterData
{
    public WaterState waterState;
}