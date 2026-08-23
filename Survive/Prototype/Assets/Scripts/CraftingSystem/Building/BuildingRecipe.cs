using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSo", menuName = "Scriptable Objects/BuildingSo")]
public class BuildingRecipe : CraftingSO
{
    public StructureType structureType;
    public bool isChildStructure;
}