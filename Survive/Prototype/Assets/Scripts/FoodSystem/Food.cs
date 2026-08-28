using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FoodSystem
{
    public class Food : Obj<ObjSo>
    {
        private int _health;
        [Header("cooking Threshold")]
        [SerializeField] public float cookTime = 10;
        [SerializeField] public float burnTime = 20;

        public FoodState CurrentState { get; set; } = FoodState.Raw;

        protected override void Awake()
        {
            canUse = true;
            Gm = gameObject;
            useMeDescription = "Eat";
            base.Awake();
        }

        public override void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(So as FoodSo);
            // remove from inventory
        }

        public void AddBurntFoodDebuff()
        {
            
        }
        protected override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = true;
        }
    }
}

public enum FoodState
{
    Raw,
    Cooked,
    Burnt
}