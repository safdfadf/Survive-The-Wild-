using Player;
using UnityEngine;

namespace FoodSystem
{
    public class WaterObj : Food
    {
        [SerializeField] private Material waterMat;
        [SerializeField] private ParticleSystem steamParticles;
        protected override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = false;
            canDisplay = false;
        }

        public override void ExecuteCooking(float deltaTime)
        {
            cookTime -= deltaTime;
            if (cookTime <= 0)
            {
               //boil;
               // remove poison probability 
               // here we simply update the data 
            }
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