using System;
using UnityEngine;

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
}
