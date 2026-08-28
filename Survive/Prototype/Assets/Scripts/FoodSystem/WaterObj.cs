using Player;
using UnityEngine;

namespace FoodSystem
{
    public class WaterObj : Obj<ObjSo>// for water object when player comes in contact only then allow 
    // ui appearence 
    {
        public WaterState waterState { get; private set; }
        [SerializeField] private FoodSo waterSo;

        protected override void Awake()
        {
            base.Awake();
            canBeCollected = false;
            rb = null;
            Gm = gameObject;
            So = waterSo;
        }

        public void SetWaterObject(WaterState state)
        {
            waterState = state;
        }

        public override void UseMe()
        {
            // trigger drink animation 
            // trigger audio 
            PlayerRepository.instance.ConsumeFood(So as FoodSo);
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