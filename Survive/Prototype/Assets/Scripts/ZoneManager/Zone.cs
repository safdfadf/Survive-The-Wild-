using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum WaterBodyType
{
    Null,
    Lake,
    River
}

public class Zone : MonoBehaviour
{
    [SerializeField] public Activity zoneType;
    [SerializeField] private List<Vector3> zonePosition = new();


    private HashSet<Vector3> occupiedPositions = new();

    private int maxPosition = 6;
    private float disBtwPos = 5;

    private TextMeshProUGUI TypeText;
    private bool _isDrinkingZone;
    private int edgeOffset = 5;
    private int _attempts = 0;
    public bool HasAvailablePosition => zonePosition.Any(pos => !occupiedPositions.Contains(pos));
    private Dictionary<int, Species> hourlyOccupancy = new(); // hour → species
    public WaterBody WaterBody { get; private set; }

    private void Awake()
    {
        WaterBody = GetComponent<WaterBody>();
        _isDrinkingZone = WaterBody != null;
        GenerateZonePosition();
        TypeText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    // all the chunks that lies in shape of this game object set it to empty chunk

    public void SetZoneText(Activity zone)
    {
        TypeText.text = zone.ToString();
    }

    private void GenerateZonePosition()
    {
        if (_isDrinkingZone)
        {
            switch (WaterBody.bodyType)
            {
                case WaterBodyType.Lake:
                    GenerateLakePos();
                    break;
                case WaterBodyType.River:
                    GenerateRiverPos();
                    break;
            }

            return;
        }

        GenerateGrndPos();
    }

    private void GenerateGrndPos()
    {
        SphereCollider collier = gameObject.GetComponent<SphereCollider>();
        if (collier == null)
        {
            collier = gameObject.AddComponent<SphereCollider>();
            collier.isTrigger = true;
        }

        int attempts = 0;
        // while (zonePosition.Count < maxPosition && attempts < maxPosition)
        for (int i = 0; i < maxPosition; i++)
        {
            Vector3 randomPosition =
                transform.position + Random.insideUnitSphere * collier.radius; // now it will incerase from the center
            randomPosition.y = transform.position.y;

            foreach (Vector3 pos in zonePosition)
            {
                float distance = Vector3.Distance(pos, randomPosition);
                if (distance < disBtwPos)
                {
                    // Push the new position away from the conflicting one
                    Vector3 direction = (randomPosition - pos).normalized;
                    randomPosition = pos + direction * disBtwPos;

                    // Make sure it’s still inside the sphere radius
                    if (Vector3.Distance(transform.position, randomPosition) > collier.radius)
                    {
                        // _tooClose = true; // reject this one
                    }
                    else
                    {
                        //_tooClose = false; // fixed by pushing away
                    }
                }
            }

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                if (!IsDuplicate(hit.position))
                    zonePosition.Add(hit.position);
            }

            attempts++;
        }
    }

    private void GenerateLakePos()
    {
        Collider _meshCollider = GetComponent<Collider>();
        Bounds bounds = _meshCollider.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        int maxPos = 10;

        for (int i = 0; i < maxPos; i++)
        {
            float t = i / (float)(maxPos - 1);

            // Sample along each edge
            List<Vector3> positions = new();
            positions.Add(new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, min.z - edgeOffset));
            positions.Add(new Vector3(Mathf.Lerp(min.x, max.x, t), min.y, max.z + edgeOffset));
            positions.Add(new Vector3(min.x - edgeOffset, min.y, Mathf.Lerp(min.z, max.z, t)));
            positions.Add(new Vector3(max.x + edgeOffset, min.y, Mathf.Lerp(min.z, max.z, t)));
            foreach (var pos in positions)
            {
                if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    zonePosition.Add(hit.position);
                }
            }
        }
    }

    public Vector3? RequestPosition()
    {
        foreach (var pos in zonePosition)
        {
            if (!occupiedPositions.Contains(pos))
            {
                occupiedPositions.Add(pos);
                _attempts = 0;
                return pos;
            }
        }

        if (_attempts > 2) return null;
        if (zonePosition.Count != maxPosition)
        {
            GenerateZonePosition();
            _attempts++;
            return RequestPosition();
        }

        return null;
    }

    public void ReleasePosition(Vector3 pos)
    {
        occupiedPositions.Remove(pos);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var pos in zonePosition)
        {
            Gizmos.DrawSphere(pos, 1f);
        }
    }

    public bool IsAvailableForTime(Species species, int startHour, int endHour)
    {
        for (int hour = startHour; hour < endHour; hour++)
        {
            if (hourlyOccupancy.TryGetValue(hour, out var occupant))
            {
                if (occupant != species)
                    return false; // conflict
            }
        }

        return true;
    }

    public void ReserveTime(Species species, int startHour, int endHour)
    {
        for (int hour = startHour; hour < endHour; hour++)
        {
            hourlyOccupancy[hour] = species;
        }
    }

    private void GenerateRiverPos()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogWarning("MeshCollider missing on river zone.");
            return;
        }

        Bounds bounds = meshCollider.bounds;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        int samplesAlongLength = 10;
        float edgeBias = 0.8f; // 0 = center, 1 = edge

        for (int i = 0; i < samplesAlongLength; i++)
        {
            float t = i / (float)(samplesAlongLength - 1);
            float x = Mathf.Lerp(min.x, max.x, t);

            // Sample near both edges by biasing toward min.z and max.z
            float zNear = Mathf.Lerp(bounds.center.z, min.z, edgeBias);
            float zFar = Mathf.Lerp(bounds.center.z, max.z, edgeBias);

            Vector3[] candidates = new[]
            {
                new Vector3(x, bounds.center.y, zNear),
                new Vector3(x, bounds.center.y, zFar)
            };

            foreach (var pos in candidates)
            {
                if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 15, NavMesh.AllAreas))
                {
                    zonePosition.Add(hit.position);
                }
            }
        }
    }

    bool IsDuplicate(Vector3 newPos)
    {
        foreach (var pos in zonePosition)
        {
            if (Vector3.Distance(pos, newPos) < 1) // or disBtwPos
                return true;
        }

        return false;
    }

    private void RefuzeEnviornment()
    {
        MeshCollider meshCol = GetComponent<MeshCollider>();
        Bounds zoneBounds = meshCol.bounds;
        ChunkRepo.instance.SetEmptyChunk(zoneBounds);
    }
}