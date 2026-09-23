using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimalSo", menuName = "Scriptable Objects/AnimalSo")]
public class AnimalSo : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    public bool isScheduled;
    public GameObject prefab;
    public RegionType regionType;
    public int minAmount;
    public int maxAmount;
    public ObjSo collectable;
    public List<GameObject> TrackMesh;
    public float spawnProbability;
    public bool isAggresive;
   

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}