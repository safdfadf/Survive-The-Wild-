using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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

[CustomEditor(typeof(CraftingSO))]
public class CraftingSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Properties
        SerializedProperty hasUpgradesProp = serializedObject.FindProperty("hasUpgrades");
        SerializedProperty upgradesProp = serializedObject.FindProperty("upgrades");

        // Draw everything except upgrades
        DrawPropertiesExcluding(serializedObject, "m_Script", "upgrades", "hasUpgrades");

        // Draw the toggle
        EditorGUILayout.PropertyField(hasUpgradesProp);

        // Conditionally show upgrades list
        if (hasUpgradesProp.boolValue)
        {
            EditorGUILayout.PropertyField(upgradesProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}