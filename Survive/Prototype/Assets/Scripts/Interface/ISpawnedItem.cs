using UnityEngine;

public interface ISpawnedItem
{
    int Amount { get; }
    GameObject Prefab { get;}
    float SpawningProbability { get; }
}
