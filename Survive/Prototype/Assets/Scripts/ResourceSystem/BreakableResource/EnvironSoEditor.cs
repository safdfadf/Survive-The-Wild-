using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironSo))]
public class EnvironSoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty canBreakProp = serializedObject.FindProperty("canBreak");
        SerializedProperty breakableProp = serializedObject.FindProperty("breakableData");

        // Draw default fields
        DrawPropertiesExcluding(serializedObject, "m_Script", "breakableData","canBreak");

        // Draw canBreak toggle
        EditorGUILayout.PropertyField(canBreakProp);

        // Conditionally show breakableData
        if (canBreakProp.boolValue)
        {
            EditorGUILayout.PropertyField(breakableProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}