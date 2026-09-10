using FoodSystem;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FoodSo", menuName = "Scriptable Objects/FoodSo")]
public class FoodSo : ObjSo
{
    [FormerlySerializedAs("CalorieCount")] public NutrientsCount nutrientsCount;
    public RegionType regionType;
    [Header("UI")] public Sprite cooked;
    public Sprite burnt;
    public Sprite rotten;
}