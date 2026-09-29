using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/ResourceData")]
public class ObjSo : ScriptableObject, ISpawnedItem
{
    [field: SerializeField] public string itemName { get; private set; }
    public GameObject prefab;
    public Sprite sprite;
    public int amount; // ToDo: replace this with range (min and max)
    public Vector2Int size;
    public float appearanceProb;
    public GameObject Prefab => prefab;
    public int Amount => amount;
    public float SpawningProbability => appearanceProb;

    private void OnValidate()
    {
#if UNITY_EDITOR
        itemName = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}