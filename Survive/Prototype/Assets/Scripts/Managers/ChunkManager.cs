using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodSystem;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct RegionSeed
{
    public RegionType regionType;
    public Transform seedPos;
}

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;
    [SerializeField] private int chunkSide = 100;
    [SerializeField] private MovementHandler player;
    [SerializeField] private List<RegionSeed> seeds;
    [SerializeField] private GameObject objTestBounds;
    public Dictionary<Vector2Int, Chunk> AllChunks { get; private set; } = new();
    public List<Chunk> activeChunks { get; private set; } = new();

    [Header("CashedPos")] [SerializeField] private int cashedPosCount = 0;

    public Bounds CurrentBounds { get; private set; } // total bounds of active chunk
    private Dictionary<RegionType, Bounds> regionBounds = new();

    private Vector2Int _playerPos;
    private Chunk _currentChunk;
    [SerializeField] private float radius = .9f;


    private RegionType _currentRegion = RegionType.Null;

    [SerializeField] private float chunkUpdateInterval = 0.5f;
    [SerializeField] private Terrain[] invalidTerrain;
    private Coroutine chunkRoutine;
    private List<Chunk> emptyChunks = new();
    // create another list or hashset ofdesired chunks these will ne neighbour chunks or active chunks 

    private HashSet<Chunk> desiredChunks = new();
    private Vector2Int previousChunk;

    private void OnEnable()
    {
        if (chunkRoutine != null)
            StopCoroutine(chunkRoutine);
    }

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

        RetPosOnNv.RemoveInvalidTerrains(invalidTerrain);
        GenerateChunks();
    }

    private void Start()
    {
        _currentChunk = GetChunkAtPos(player.transform.position);
        chunkRoutine = StartCoroutine(ChunkUpdateRoutine());
    }

    private IEnumerator ChunkUpdateRoutine()
    {
        while (true) // this update every frame
        {
            Vector2Int currentChunk =
                GetChunkIndex(player.transform.position);

            if (currentChunk != previousChunk)
            {
                previousChunk = currentChunk;
                UpdateDesiredChunks(currentChunk);
            }

            yield return new WaitForSeconds(chunkUpdateInterval);
        }
    }


    private void UpdateDesiredChunks(Vector2Int playerChunk)
    {
        desiredChunks.Clear();

        var chunkRadius = Mathf.CeilToInt(radius);

        for (var x = -chunkRadius; x <= chunkRadius; x++)
        {
            for (var z = -chunkRadius; z <= chunkRadius; z++)
            {
                Vector2Int index = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + z);

                if (AllChunks.TryGetValue(index, out var chunk))
                {
                    desiredChunks.Add(chunk);
                }
            }
        }

        foreach (Chunk chunk in desiredChunks)
        {
            if (activeChunks.Contains(chunk))
                continue;

            chunk.isActive = true;

            activeChunks.Add(chunk);
            activeChunks.Add(chunk);

            SpawnChunk(chunk);
            ActivateTracks(chunk);

            EventBus.OnChunkChanged?.Invoke(chunk);
            EventBus.OnGpuDeactivateInChunk?.Invoke(chunk);
        }

        // Deactivate chunks no longer desired
        List<Chunk> chunksToRemove = new();

        foreach (Chunk chunk in activeChunks)
        {
            if (desiredChunks.Contains(chunk))
                continue;

            chunk.isActive = false;

            DespawnChunk(chunk);

            chunksToRemove.Add(chunk);
        }

        foreach (Chunk chunk in chunksToRemove)
        {
            activeChunks.Remove(chunk);
            activeChunks.Remove(chunk);
        }

        UpdateCurrentBounds();
    }

    private void GenerateChunks()
    {
        Terrain[] terrains = Terrain.activeTerrains;
        // Compute world bounds
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (Terrain t in terrains)
        {
            Vector3 pos = t.transform.position;
            Vector3 size = t.terrainData.size;

            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x + size.x);

            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z + size.z);
        }

        float worldWidth = maxX - minX;
        float worldDepth = maxZ - minZ;

        int chunksX = Mathf.CeilToInt(worldWidth / chunkSide);
        int chunksZ = Mathf.CeilToInt(worldDepth / chunkSide);
        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                Vector3 center = new Vector3(x * chunkSide + chunkSide / 2, 0, z * chunkSide + chunkSide / 2);
                Bounds bounds = new Bounds(center, new Vector3(chunkSide, 0, chunkSide));

                Chunk chunk = new Chunk
                {
                    index = new Vector2Int(x, z),
                    bounds = bounds,
                    regionType = GetRegionTypeForChunk(center),
                    isActive = false
                };
                for (int i = 0; i < cashedPosCount; i++)
                {
                    var pos = RetPosOnNv.GetRandomPosOnTerrain(chunk.bounds);
                    chunk.cashedPos.Add(new PosInChunk(pos));
                }

                AllChunks.Add(chunk.index, chunk);
            }
        }

        GenerateRegionBound();
    }

    private Vector2Int GetChunkIndex(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkSide);
        int z = Mathf.FloorToInt(pos.z / chunkSide);
        return new Vector2Int(x, z);
    }

    private RegionType GetRegionTypeForChunk(Vector3 chunkCenter)
    {
        if (seeds.Count == 0) return RegionType.Forest;
        float minDist = float.MaxValue;
        RegionType closest = RegionType.Forest;

        foreach (var seed in seeds)
        {
            float dis = Vector3.Distance(chunkCenter, seed.seedPos.position);
            if (dis < minDist)
            {
                minDist = dis;
                closest = seed.regionType;
            }
        }

        return closest;
    }

    public bool PosLiesInActiveChunk(Vector3 pos)
    {
        foreach (var chunk in activeChunks)
        {
            if (chunk.bounds.Contains(pos))
            {
                return true;
            }
        }

        return false;
    }

    private void ActivateNearbyChunks()
    {
        List<Chunk> chunksToActivate = new List<Chunk>();

        foreach (var kvp in AllChunks)
        {
            var chunk = kvp.Value;
            int dx = Mathf.Abs(chunk.index.x - _playerPos.x);
            int dz = Mathf.Abs(chunk.index.y - _playerPos.y);

            if (dx <= radius && dz <= radius)
            {
                chunksToActivate.Add(chunk);
            }
        }

        chunksToActivate = chunksToActivate
            .OrderBy(c => Vector3.Distance(player.transform.position, c.bounds.center))
            .ToList();

        int allowed = 6;

        for (int i = 0; i < chunksToActivate.Count; i++)
        {
            Chunk chunk = chunksToActivate[i];

            if (i < allowed)
            {
                // Should be active
                if (!chunk.isActive)
                {
                    chunk.isActive = true;
                    activeChunks.Add(chunk);
                    SpawnChunk(chunk);
                    ActivateTracks(chunk);
                    EventBus.OnChunkChanged?.Invoke(chunk);
                    EventBus.OnGpuDeactivateInChunk.Invoke(chunk);
                }
            }
            else
            {
                // Should be deactivated
                if (chunk.isActive)
                {
                    chunk.isActive = false;
                    activeChunks.Remove(chunk);
                    DespawnChunk(chunk);
                }
            }
        }

        GpuInstancing();
        UpdateCurrentBounds();
    }

    private void UpdateCurrentBounds()
    {
        Bounds mergedBounds = new Bounds();
        bool first = true;

        foreach (Chunk chunk in activeChunks)
        {
            if (first)
            {
                mergedBounds = chunk.bounds; // initialize with first active chunk
                first = false;
            }
            else
            {
                mergedBounds.Encapsulate(chunk.bounds); // expand to include this chunk
            }
        }

        CurrentBounds = mergedBounds;
    }

    private void GpuInstancing()
    {
        foreach (var chunks in AllChunks.Values)
        {
            if (!chunks.isActive) // all the chunks that are in active are 
            {
                //  EventBus.OnGpuActivateInChunk?.Invoke(chunks);
            }
        }
    }

    private void SpawnChunk(Chunk chunk)
    {
        if (_currentRegion == RegionType.Null)
        {
            _currentRegion = chunk.regionType;
        }

        if (chunk.isEmptyChunk) return;
        _currentRegion = chunk.regionType;
        EventBus.CreateAnimalData.Invoke(_currentRegion, GetTestBounds()); // create animal data

        GenericSpawner.Instance.SpawnInChunk<EnvironSo, Environment>(
            SoProvider.instance.GetEnvironmentSo(chunk.regionType), chunk, chunk.objectInChunk);
        GenericSpawner.Instance.SpawnInChunk<ObjSo, Obj<ObjSo>>(SoProvider.instance.GetResourceSo(),
            chunk, chunk.objectInChunk);
    }

    private void OnDrawGizmos()
    {
        foreach (var chunk in activeChunks)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                chunk.bounds.center,
                new Vector3(chunk.bounds.size.x, 1f, chunk.bounds.size.z)
            );
        }
    }

    private void GenerateRegionBound()
    {
        foreach (var kvp in AllChunks)
        {
            var chunk = kvp.Value;
            if (!regionBounds.ContainsKey(chunk.regionType))
            {
                regionBounds[chunk.regionType] = chunk.bounds;
            }
            else
            {
                Bounds current = regionBounds[chunk.regionType];
                current.Encapsulate(chunk.bounds);
                regionBounds[chunk.regionType] = current;
            }
        }
    }

    private Bounds GetRegionBound(RegionType regionType)
    {
        return regionBounds[regionType];
    }

    private void DespawnChunk(Chunk chunk) // with current set up we need to deactivate individually 
    {
        EventBus.OnDeactiveChunk?.Invoke(chunk); // Deactivate Animal 
        List<GameObject> environ = new();
        List<GameObject> resources = new();
        List<GameObject> food = new();

        TrackHandler.instance.ReturnTracks(chunk);
        foreach (var obj in chunk.objectInChunk)
        {
            if (obj == null)
            {
                Debug.Log(obj.gameObject.name);
                continue;
            }

            if (obj.TryGetComponent<Environment>(out var envObj))
                environ.Add(envObj.gameObject);
            if (obj.TryGetComponent<Obj<ObjSo>>(out var resourceObj))
                resources.Add(resourceObj.gameObject);
            if (obj.TryGetComponent<Food>(out var foodObj))
                food.Add(foodObj.gameObject);
        }

        GenericSpawner.Instance.DespawnChunk(chunk, environ, GetPrefab);
        GenericSpawner.Instance.DespawnChunk(chunk, resources, GetPrefab);
        GenericSpawner.Instance.DespawnChunk(chunk, food, GetPrefab);
        environ.Clear();
        resources.Clear();
        food.Clear();
    }

    private GameObject GetPrefab(GameObject obj)
    {
        if (obj.TryGetComponent<Environment>(out var envObj))
        {
            EnvironSo so = envObj.environSo;
            if (so == null)
            {
                Debug.Log("so was null");
                return null;
            }

            return so.prefab;
        }
        else if (obj.TryGetComponent<Obj<ObjSo>>(out var resObj))
        {
            ObjSo so = resObj.So;
            return so.prefab;
        }
        else if (obj.TryGetComponent<Food>(out var foodObj))
        {
            ObjSo so = foodObj.So;
            return so.prefab;
        }

        return null;
    }

    public Chunk GetChunkAtPos(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkSide);
        int y = Mathf.FloorToInt(pos.z / chunkSide);

        Vector2Int index = new Vector2Int(x, y);

        if (AllChunks.TryGetValue(index, out Chunk chunk))
            return chunk;

        return null;
    }

    public Bounds GetActiveChunkBounds()
    {
        return CurrentBounds;
    }

    public bool IsPosInPlayerChunk(Vector3 pos)
    {
        Chunk chunk = GetChunkAtPos(player.transform.position);
        if (chunk.bounds.Contains(pos))
            return true;
        return false;
    }

    public bool IsPlayerInChunk(Chunk chunk)
    {
        return chunk.bounds.Contains(player.transform.position);
    }

    public bool ObjectLiesInActiveChunk(Vector3 pos)
    {
        foreach (var chunk in activeChunks)
        {
            if (chunk.bounds.Contains(pos)) return true;
        }

        return false;
    }

    public void ActivateTracks(Chunk chunk)
    {
        List<TrackData> trackData = chunk.TrackData;

        foreach (var data in trackData)
        {
            GameObject so = data.soAnimal.TrackMesh[Random.Range(0, data.soAnimal.TrackMesh.Count)];
            data.prefab = so;
            SpawnTracksWithDelay(so, data, chunk);
            GameObject obj = GlobalPool.instance.Get(so, data.pos);
            Tracks track = obj.GetComponent<Tracks>();
            track.Initialize(data);
            chunk.activeTracks.Add(obj);
        }
    }

    private IEnumerator SpawnTracksWithDelay(GameObject so, TrackData data, Chunk chunk)
    {
        yield return new WaitForSeconds(2f);
        var obj = GlobalPool.instance.Get(so, data.pos);
        var track = obj.GetComponent<Tracks>();
        track.Initialize(data);
        chunk.activeTracks.Add(obj);
        Debug.Log(chunk.activeTracks.Count);
    }

    public Vector3 GetClosestInactiveChunkPosition(Vector3 animalPos) // returns a pos from closest inactive chunk
    {
        Chunk closest = null;
        float bestDist = float.MaxValue;

        foreach (Chunk chunk in AllChunks.Values)
        {
            if (activeChunks.Contains(chunk))
                continue; // skip active chunks

            float dist = Vector3.Distance(animalPos, chunk.bounds.center);

            if (dist < bestDist)
            {
                bestDist = dist;
                closest = chunk;
            }
        }

        if (closest == null) return animalPos;
        foreach (var posInChunk in closest.cashedPos)
        {
            if (posInChunk.IsAvailable)
                return posInChunk.Position;
        }

        return animalPos;
    }

/*
    private void OnDrawGizmos()
    {
        if (AllChunks == null || AllChunks.Count == 0)
            return;

        foreach (var kvp in AllChunks)
        {
            Chunk chunk = kvp.Value;

            Gizmos.color = chunk.isActive ? Color.red : Color.gray;

            Gizmos.DrawWireCube(
                chunk.bounds.center,
                new Vector3(chunk.bounds.size.x, 1, chunk.bounds.size.z)
            );
        }

        foreach (var chunk in emptyChunks)
        {
            Gizmos.color =  Color.green;

            Gizmos.DrawWireCube(
                chunk.bounds.center,
                new Vector3(chunk.bounds.size.x, 1f, chunk.bounds.size.z)
            );
        }

        if (regionBounds == null || regionBounds.Count == 0)
            return;

        DrawRegions();
    }
    */

    private void DrawRegions()
    {
        foreach (var kvp in regionBounds)
        {
            RegionType region = kvp.Key;
            Bounds bounds = kvp.Value;

            Gizmos.color = GetRegionColor(region);

            Gizmos.DrawWireCube(
                bounds.center,
                new Vector3(bounds.size.x, 2f, bounds.size.z)
            );
        }
    }

    public Vector3 GetAvailablePosinChunk(Chunk chunk)
    {
        foreach (var pos in chunk.cashedPos)
        {
            if (pos.IsAvailable)
                return pos.Position;
        }

        return new Vector3(0, 0, 0);
    }

    private Color GetRegionColor(RegionType region)
    {
        return region switch
        {
            RegionType.Forest => Color.green,
            RegionType.Swamp => Color.cyan,
            RegionType.Sawana => Color.yellow,
            _ => Color.white
        };
    }

    private Bounds GetTestBounds() // used to spawn animal and its zone in limited space in a 
    {
        Collider col = objTestBounds.GetComponent<Collider>();
        return col.bounds;
    }

    public void SetEmptyChunk(Bounds bounds)
    {
        foreach (var chunk in AllChunks.Values)
        {
            if (chunk.bounds.SqrDistance(bounds.center) <= bounds.extents.magnitude * bounds.extents.magnitude)
            {
                chunk.isEmptyChunk = true;
            }
        }

        foreach (var c in AllChunks.Values)
        {
            if (c.isEmptyChunk)
                emptyChunks.Add(c);
        }
    }

    public void CheckINChunkPOsLies(Vector3 pos)
    {
        foreach (var chunk in AllChunks.Values)
        {
            if (!chunk.bounds.Contains(pos)) continue;
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = chunk.bounds.center;
            obj.transform.localScale = new Vector3(1, 1, 1);
            return;
        }
        print("this lies no where ");
    }
}