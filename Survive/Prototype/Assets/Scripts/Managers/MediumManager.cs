using System;
using UnityEngine;

public class MediumManager : MonoBehaviour
{
    public static MediumManager Instance;
    private ChunkManager chunkManager;
    private AnimalSpawner _animalSpawner;
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
        chunkManager = FindAnyObjectByType<ChunkManager>();
       _animalSpawner = FindAnyObjectByType<AnimalSpawner>();
        
    }
    public bool OutofBoundsCheck(Vector3 position)
    {
        if(chunkManager.ObjectLiesInActiveChunk(position))return true;
        return false;
    }

    
}
