using UnityEngine;

[CreateAssetMenu(fileName = "FoodSo", menuName = "Scriptable Objects/FoodSo")]
public class FoodSo : ObjSo
{
    public float calories;
    public float proteinCount;
    public float carbonCount;
    public float fatCount;
    public float hydrationCount;
    public RegionType regionType;
}
