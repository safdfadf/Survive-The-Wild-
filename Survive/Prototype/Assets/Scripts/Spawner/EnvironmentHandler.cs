using System.Collections.Generic;
using UnityEngine;

public class EnvironmentHandler : MonoBehaviour
{
    public void SpawnEnvironmentInChunk(List<EnvironSo> environList, Chunk chunk)
    {
   
        foreach (var so in environList)
        {
            SpawnSingleEnvironmentType(so, chunk);
        }
    }
    private void SpawnSingleEnvironmentType(EnvironSo so, Chunk chunk)
    {
        int needed = so.amount;
        // 1. Reuse from pool
        for (int i = 0; i < needed; i++)
        {
            if (i >= chunk.cashedPos.Count)
                break;

            var posInChunk = chunk.cashedPos[i];
            if (!posInChunk.IsAvailable)
                continue;

            GameObject obj = GlobalPool.instance.Get(so.prefab, posInChunk.Position);
            posInChunk.IsAvailable = false;

            chunk.objectInChunk.Add(obj);

            var env = obj.GetComponent<Environment>();
           
        }
    }
    public void DespawnEnvironment(Chunk chunk)
    { 
      //  Debug.Log("Despawning Chunk ");
      for(int i=0 ; i< chunk.objectInChunk.Count-1; i--)
        {
            var obj = chunk.objectInChunk[i];
            if (obj == null)
            {
                chunk.objectInChunk.RemoveAt(i);
                continue;
            }
        }

        foreach (var cashedPo in chunk.cashedPos)// cashed pos list 2 
        {
            cashedPo.IsAvailable = true;
        }
    }
}

