using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnvironSo", menuName = "Scriptable Objects/EnvironSo")]
public class EnvironSo : ScriptableObject, ISpawnedItem
{
    public string id;
    public GameObject prefab;
    public int amount;
    public bool canBreak;
    public BreakableObjects breakableData;
    public RegionType regionType;
    public float appearanceProb;
    public int Amount => amount;

    public GameObject Prefab => prefab;
    public float SpawningProbability => appearanceProb;
}

[System.Serializable]
public class BreakableObjects
{
    public int Health;
    public ResourceSo resourceSo;
}