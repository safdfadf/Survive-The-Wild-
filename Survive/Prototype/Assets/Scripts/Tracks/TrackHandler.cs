using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrackHandler : MonoBehaviour // no need to be monobehaviour 
{
    [SerializeField] private int maxTrackAgeHours;
    private ChunkManager chunkManager;
    private GameObject prefab;
    public static TrackHandler instance;
    private void Awake()
    {
        chunkManager = FindAnyObjectByType<ChunkManager>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    private void Start()
    {
        prefab = SoProvider.instance.GetTrack();
    }

    private void OnEnable()
    {
        EventBus.OnHourChanged += AgeTrackData;
    }
    private void OnDisable()
    {
        EventBus.OnHourChanged -= AgeTrackData;
    }
    public void CreateTracks(MovementSegment segment, Species specie,AnimalState state,AnimalSo so)
    {
        // we need to make sure it does not creates unecessary tracks 
        float distance = Vector3.Distance(segment.StartPos, segment.EndPos);
        float spacing = 50f; // later species-specific
        int steps = Mathf.CeilToInt(distance / spacing);
        int maxtracksPerChunk = Random.Range(3, 5);
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)steps;
            Vector3 pos = Vector3.Lerp(segment.StartPos, segment.EndPos, t);
            Vector3 dir = (segment.EndPos - segment.StartPos).normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            string Dir = GetCompassDirection(angle);
            
            // Adjust to terrain height
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            TrackData data = new TrackData(so,pos, specie, 0,false,maxTrackAgeHours,Dir,angle ,state);
           Chunk chunk =  chunkManager.GetChunkAtPos(pos);
           if(chunk == null){Debug.Log("Could not find chunk");return;}
           if(chunk.TrackData.Count >= maxtracksPerChunk) continue;
           chunk.TrackData.Add(data);
           if (chunk.isActive)
           {
               chunkManager.ActivateTracks(chunk);// activate the tracks if chunk is active
           }
        }
    } 
    private void AgeTrackData(int hour)
    {
        // all the data in active tracks is being updated 
        foreach (var chunk in chunkManager.AllChunks.Values)
        {
            for (int i = chunk.TrackData.Count - 1; i >= 0; i--)
            {
                chunk.TrackData[i].TimeStamp += 1f;
                chunk.TrackData[i].UpdateTrackAge();

                if (chunk.TrackData[i].TimeStamp > maxTrackAgeHours)
                {
                    chunk.TrackData.RemoveAt(i);
                }
            }
        }
    }
    public void ReturnTracks(Chunk chunk)
    {
        foreach (var track in chunk.activeTracks)
        {
            GlobalPool.instance.Return(prefab, track);
        }
        chunk.activeTracks.Clear();
    }
    private string GetCompassDirection(float angle)
    {
        if (angle < 0) angle += 360f;

        if (angle < 22.5f) return "North";
        if (angle < 67.5f) return "North-East";
        if (angle < 112.5f) return "East";
        if (angle < 157.5f) return "South-East";
        if (angle < 202.5f) return "South";
        if (angle < 247.5f) return "South-West";
        if (angle < 292.5f) return "West";
        return "North-West";
    }
}
public enum TrackAge
{
   VeryFresh, Fresh, Recent,Old
}