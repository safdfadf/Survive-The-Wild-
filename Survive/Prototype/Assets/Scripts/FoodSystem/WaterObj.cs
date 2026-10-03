using Player;
using UnityEngine;

namespace FoodSystem
{
    public class WaterObj : Food
    {
        [SerializeField] private Material waterMat;

        public override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = false;
            canDisplay = false;
        }
        // should i let it handle it boiling 
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