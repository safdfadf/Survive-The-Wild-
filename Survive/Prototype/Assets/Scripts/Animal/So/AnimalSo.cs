using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/AnimalSo")]
public class AnimalSo : ScriptableObject
{
    //ToDo Remove use of specie 
    public Species specie;
    public bool isScheduled;
    public GameObject prefab;
    public RegionType regionType;
    public int minAmount;
    public int maxAmount;
    [FormerlySerializedAs("resourceSo")] public ObjSo objSo;// drop the resource 
    public List<GameObject> TrackMesh;
    public float spawnProbability;
    public bool isAggresive;
    public int damage;
}
