using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/AnimalSo")]
public class AnimalSo : ScriptableObject
{
    public Species specie;
    public GameObject prefab;
    public RegionType regionType;
    public int minAmount;
    public int maxAmount;
    public ResourceSo resourceSo;// drop the resource 
    public List<GameObject> TrackMesh;
}
