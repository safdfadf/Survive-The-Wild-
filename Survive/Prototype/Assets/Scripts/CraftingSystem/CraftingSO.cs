using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CraftingSO", menuName = "Scriptable Objects/CraftingSO")]
public class CraftingSO : ScriptableObject 
{
    public Ingredient[] ingredients;
    [FormerlySerializedAs("So")] public ResourceSo resSo;
}
