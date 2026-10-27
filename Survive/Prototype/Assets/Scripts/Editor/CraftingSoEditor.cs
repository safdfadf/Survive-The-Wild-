using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CraftingSO))]
public class CraftingSoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var hasUpgradesProp = serializedObject.FindProperty("hasUpgrades");
        var upgradesProp = serializedObject.FindProperty("upgrades");

        DrawPropertiesExcluding(serializedObject, "m_Script", "upgrades", "hasUpgrades");
        EditorGUILayout.PropertyField(hasUpgradesProp);

        if (hasUpgradesProp.boolValue)
        {
            EditorGUILayout.PropertyField(upgradesProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}