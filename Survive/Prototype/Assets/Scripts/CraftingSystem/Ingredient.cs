using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Ingredient
{
    [FormerlySerializedAs("resourceSo")] public ObjSo objSo;
    public int amount;
}
// instead of resource Type we just check 
