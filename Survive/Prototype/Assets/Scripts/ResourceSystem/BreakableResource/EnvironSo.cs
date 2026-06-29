using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EnvironSo", menuName = "Scriptable Objects/EnvironSo")]
public class EnvironSo : ScriptableObject,ISpawnedItem
{
    public string id;
    public GameObject prefab;
    public int amount;
    public int health;
    public RegionType regionType;
    public ResourceSo resourceSo;
    public float appearanceProb;
    public int Amount => amount;
    
    public GameObject Prefab => prefab;
    public float SpawningProbability => appearanceProb;

}
