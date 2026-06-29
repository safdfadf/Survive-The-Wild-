using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/ResourceData")]
public class ResourceSo : ScriptableObject,ISpawnedItem
{
   
    public GameObject prefab;
    public int amount;
    public Vector2Int size;
    public float appearanceProb;
    public GameObject Prefab => prefab;
    public int Amount => amount;
    public float SpawningProbability => appearanceProb;
}
