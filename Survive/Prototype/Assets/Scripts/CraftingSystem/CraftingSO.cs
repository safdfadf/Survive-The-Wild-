using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CraftingSO", menuName = "Scriptable Objects/CraftingSO")]
public class CraftingSO : ScriptableObject //ToDo: rename  recipeSo 
{
    public Ingredient[] ingredients;
    public ObjSo resSo;

    public bool hasUpgrades;

    public List<CraftingSO> upgrades;
    // we are storing upgrades here , but we also need data for locked and 
}

public class RecipeData
{
    public CraftingSO craftingSo;
    public bool isLocked;
    public List<RecipeData> upgrades = new();

    public RecipeData(CraftingSO so)
    {
        craftingSo = so;
        isLocked = true;
    }
}