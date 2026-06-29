using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public static class RetPosOnNv
{
    public static IEnumerator  GenerateCachedPositions(Chunk chunk, int count)
    {
        Bounds bounds = chunk.bounds;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = ReturnRandomNavMeshPos(bounds);

            if (pos != Vector3.zero )
            {
                chunk.cashedPos.Add(new PosInChunk(pos));
            }
            // Yield every few iterations to avoid FPS spikes
            if (i % 5 == 0)
                yield return null;
        }
    }

    public static Vector3 ReturnRandomNavMeshPos( Bounds regionBounds)
    {
        float maxY = Terrain.activeTerrain != null
            ? Terrain.activeTerrain.terrainData.size.y + Terrain.activeTerrain.transform.position.y
            : 200f;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            float x = Random.Range(regionBounds.min.x, regionBounds.max.x);
            float z = Random.Range(regionBounds.min.z, regionBounds.max.z);

            // start above the world, then sample nearest navmesh
            Vector3 samplePoint = new Vector3(x, maxY, z);

            if (NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, maxY, NavMesh.AllAreas))
            {
                // ignore Y when checking bounds, since bounds is 3D
                Vector3 p = hit.position;
                p.y = regionBounds.center.y; // temporary for bounds check
                if (regionBounds.Contains(p))
                    return hit.position;
            }
        }

      //  Debug.Log($"Failed to find valid NavMesh position inside bounds: {regionBounds}");
        return regionBounds.center;
    }

    public static Vector3 GetRandomPosOnTerrain(Bounds regionBounds)
    {
        float x = Random.Range(regionBounds.min.x, regionBounds.max.x);
        float z = Random.Range(regionBounds.min.z, regionBounds.max.z);
        
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
        y+= Terrain.activeTerrain.transform.position.y;
        return new Vector3(x, y, z);
    }

    
}
