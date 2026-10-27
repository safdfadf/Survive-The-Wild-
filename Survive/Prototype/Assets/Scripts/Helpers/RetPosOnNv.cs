using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class RetPosOnNv
{
    private static List<Terrain> validTerrains = new();

    public static void RemoveInvalidTerrains(Terrain[] invalidTerrains)
    {
        foreach (var t in Terrain.activeTerrains)
        {
            validTerrains.Add(t);
        }

        foreach (var t in invalidTerrains)
        {
            if (validTerrains.Contains(t))
                validTerrains.Remove(t);
        }
    }

    public static IEnumerator GenerateCachedPositions(Chunk chunk, int count)
    {
        Bounds bounds = chunk.bounds;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = ReturnRandomNavMeshPos(bounds);

            if (pos != Vector3.zero)
            {
                chunk.cashedPos.Add(new PosInChunk(pos));
            }

            // Yield every few iterations to avoid FPS spikes
            if (i % 5 == 0)
                yield return null;
        }
    }

    // when getting a random mesh i need to remove specific terrain from active terrain list; 
    public static Vector3 ReturnRandomNavMeshPos(Bounds regionBounds)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            float x = Random.Range(regionBounds.min.x, regionBounds.max.x);
            float z = Random.Range(regionBounds.min.z, regionBounds.max.z);
            Terrain selectedTerrain = GetCurrentTerrain(x, z);
            // start above the world, then sample nearest navmesh
            //    float y = selectedTerrain.SampleHeight(new Vector3(x, 0, z));
            var y = selectedTerrain != null
                ? selectedTerrain.terrainData.size.y + selectedTerrain.transform.position.y
                : 200f;
            Vector3 samplePoint = new Vector3(x, y, z);

            if (NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, y, NavMesh.AllAreas))
            {
                // ignore Y when checking bounds, since bounds is 3D
                Vector3 p = hit.position;
                p.y = regionBounds.center.y; // temporary for bounds check
                if (regionBounds.Contains(p))
                {
//                    Debug.Log(hit.position);
                    return hit.position;
                }
            }
        }

        //  Debug.Log($"Failed to find valid NavMesh position inside bounds: {regionBounds}");
        Debug.Log(regionBounds.center);
        return regionBounds.center;
    }

    public static bool TryGetNavMeshPoint(Vector3 target, out Vector3 navPoint)
    {
        NavMeshHit hit;
        float maxY = Terrain.activeTerrain != null
            ? Terrain.activeTerrain.terrainData.size.y + Terrain.activeTerrain.transform.position.y
            : 200f;
        if (NavMesh.SamplePosition(target, out hit, maxY, NavMesh.AllAreas))
        {
            navPoint = hit.position;
            return true;
        }

        navPoint = Vector3.zero;
        return false;
    }

    public static Vector3 GetRandomPosOnTerrain(Bounds regionBounds)
    {
        var x = Random.Range(regionBounds.min.x, regionBounds.max.x);
        var z = Random.Range(regionBounds.min.z, regionBounds.max.z);

        List<Terrain> terrains = validTerrains;

        Terrain selectedTerrain = null;

        foreach (var terrain in terrains)
        {
            var tPos = terrain.transform.position;
            var tSize = terrain.terrainData.size;

            // Check if x,z lies inside terrain bounds
            if (x >= tPos.x && x <= tPos.x + tSize.x &&
                z >= tPos.z && z <= tPos.z + tSize.z)
            {
                selectedTerrain = terrain;
                break;
            }
        }

        if (selectedTerrain == null)
        {
            return new Vector3(x, 0, z);
        }

        var y = 0f;

        y = selectedTerrain.SampleHeight(new Vector3(x, 0, z))
            + selectedTerrain.transform.position.y;
        return new Vector3(x, y, z);
    }

    private static Terrain GetCurrentTerrain(float x, float z)
    {
        foreach (var terrain in validTerrains)
        {
            var tPos = terrain.transform.position;
            var tSize = terrain.terrainData.size;

            // Check if x,z lies inside terrain bounds
            if (x >= tPos.x && x <= tPos.x + tSize.x &&
                z >= tPos.z && z <= tPos.z + tSize.z)
            {
                return terrain;
            }
        }

        return null;
    }
}