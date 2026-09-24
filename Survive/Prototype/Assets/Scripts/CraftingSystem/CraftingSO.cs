using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CraftingSO", menuName = "Scriptable Objects/CraftingSO")]
public class CraftingSO : ScriptableObject//ToDo: rename  recipeSo 
 {
    public Ingredient[] ingredients;
    public ObjSo resSo;
}
 
public class RecipeData
{
    public CraftingSO craftingSo;
    public bool isLocked;

    public RecipeData(CraftingSO so)
    {
        craftingSo = so;
        isLocked = true;
    }
}
  