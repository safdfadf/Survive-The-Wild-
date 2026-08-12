using System.Collections.Generic;
using UnityEngine;

public class GenericSpawner : MonoBehaviour
{
    public static GenericSpawner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnInChunk<TSo, Tcomponent>(List<TSo> soList, Chunk chunk, List<GameObject> targetList)
        where Tcomponent : MonoBehaviour, IsoInitializer<TSo>
        where TSo : ISpawnedItem
    {
        foreach (var so in soList)
        {
            bool canSpawn = Random.value <= so.SpawningProbability;
            if (!canSpawn) continue;
            int alreadyPresent = 0;
            foreach (var pos in chunk.cashedPos)
            {
                if (pos.LastSpawnedSo == null) continue;
                if (pos.LastSpawnedSo.Prefab == so.Prefab && !pos.IsAvailable && pos.IsPersistent)
                {
                    alreadyPresent++;
                    GameObject go = GlobalPool.instance.Get(pos.LastSpawnedSo.Prefab, pos.Position);
                    pos.IsAvailable = false;
                    targetList.Add(go);
                }
            }

            int spawned = so.Amount - alreadyPresent;

            foreach (var pos in chunk.cashedPos)
            {
                if (spawned <= 0) break;
                if (!pos.IsAvailable || pos.LastSpawnedSo != null)
                    continue;

                GameObject obj = GlobalPool.instance.Get(so.Prefab, pos.Position);

                var component = obj.GetComponent<Tcomponent>();
                component.Initialize(so);
                component.SeCashedPos(pos);
                pos.IsAvailable = false;
                pos.LastSpawnedSo = so;
                pos.IsPersistent = true;
                targetList.Add(obj);
                spawned--;
            }
        }
    }

    public void DespawnChunk(Chunk chunk, List<GameObject> objList,
        System.Func<GameObject, GameObject> getPrefab) // if we dont have single list for all objects this wont work 
    {
        foreach (var obj in objList)
        {
            if (obj == null) continue;
            chunk.objectInChunk.Remove(obj);
            GlobalPool.instance.Return(getPrefab(obj), obj);
        }

        objList.Clear();
    }
}