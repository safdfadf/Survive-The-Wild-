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
    public ResourceSo resourceSo;// drop the resource 
    public List<GameObject> TrackMesh;
    public float spawnProbability;
}
