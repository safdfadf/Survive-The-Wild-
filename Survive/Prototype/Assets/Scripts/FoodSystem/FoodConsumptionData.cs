using DefaultNamespace;

namespace FoodSystem
{
    public class FoodConsumptionData
    {
        public NutrientsCount NutrientsCount; 
        public int SanityEffect = 0;
        public SelfAttack SelfAttack;

    }
}

[System.Serializable]
public class NutrientsCount
{
    public float calories;
    public float protien;
    public float carb;
    public float hydration;
    public float fat;
}