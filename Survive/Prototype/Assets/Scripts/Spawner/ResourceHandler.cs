using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceHandler : MonoBehaviour // in every spawner need a 
{
    // this will responsible for spawining variour resources like wood, stone, metal, tree, boulder 
    public void SpawnResourcesInChunk(List<ResourceSo> resourceList, Chunk chunk)
    {
        foreach (var ResourceSo in resourceList)
        {
           // SpawnResourceInChunk(ResourceSo, chunk);
        }
    }

    private void SpawnResourceInChunk(ResourceSo so, Chunk chunk)
    {
        int spawned = 0;
       
        foreach (var posInChunk in chunk.cashedPos)
        {
            if (spawned >= so.amount)
                break;
            if(!posInChunk.IsAvailable)continue;
            
            GameObject obj = GlobalPool.instance.Get(so.prefab, posInChunk.Position);
            posInChunk.IsAvailable = false;
            chunk.objectInChunk.Add(obj);
            

            var resource = obj.GetComponent<BaseResource>();
            resource.Initialize(so);

            spawned++;
        }
    }
    
    public void DeSpawnResources(Chunk chunk)
    {
        foreach (var obj in chunk.objectInChunk)
        {
            if (obj == null)continue;
         var so = obj.GetComponent<BaseResource>().So;
         GlobalPool.instance.Return(so.prefab,obj);
        }
        foreach (var cashedPo in chunk.cashedPos)
        {
            cashedPo.IsAvailable = true;
        }
        chunk.objectInChunk.Clear();

    }
}