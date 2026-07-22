using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk 
{
    public Vector2Int index;       // grid index
    public Bounds bounds;          // world bounds
    public RegionType regionType;  // assigned via Voronoi
    public bool isActive;
    public bool isEmptyChunk;// used by water bodies

    public List<GameObject> objectInChunk = new();
    public List<PosInChunk> cashedPos= new();
    public List<TrackData> TrackData = new();
    public List<GameObject> activeTracks = new();
}

public class PosInChunk
{
    public Vector3 Position;
    public bool  IsAvailable;
    public ISpawnedItem LastSpawnedSo;
    public bool IsPersistent;
    

    public PosInChunk(Vector3 pos)
    {
        Position = pos;
        IsAvailable = true;
    }
}