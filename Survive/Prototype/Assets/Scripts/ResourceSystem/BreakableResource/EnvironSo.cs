using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

[CreateAssetMenu(fileName = "EnvironSo", menuName = "Scriptable Objects/EnvironSo")]
public class EnvironSo : ScriptableObject, ISpawnedItem
{
    public string id;
    public GameObject prefab;
    public int amount;
    public bool canBreak;
    public BreakableObjects breakableData;
    public RegionType regionType;

    [FormerlySerializedAs("appearanceProb")]
    public float spawnProb;

    public int Amount => amount;

    public GameObject Prefab => prefab;
    public float SpawningProbability => spawnProb;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        EditorUtility.SetDirty(this);
#endif
    }
}

[System.Serializable]
public class BreakableObjects
{
    public int Health;
    [FormerlySerializedAs("resourceSo")] public ObjSo objSo;
}