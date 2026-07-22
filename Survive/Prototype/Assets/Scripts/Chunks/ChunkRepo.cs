using System;
using System.Collections.Generic;
using UnityEngine;

// TO Do: This script will be removed and scene root will be added 
public class ChunkRepo : MonoBehaviour 
{
    public static ChunkRepo instance;
    private ChunkManager _chunkManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        _chunkManager = GetComponent<ChunkManager>();
    }

    public Vector3 GetPosOutOfActiveChunk(Vector3 pos)
    {
        return _chunkManager.GetClosestInactiveChunkPosition(pos);
    }

    public bool CheckPosInActiveChunk(Vector3 pos)
    {
        return _chunkManager.PosLiesInActiveChunk(pos);
    }

    public void SetEmptyChunk(Bounds bounds)
    {
        _chunkManager.SetEmptyChunk(bounds);
    }
}
