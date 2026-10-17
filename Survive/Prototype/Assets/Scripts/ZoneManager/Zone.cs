using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public enum WaterBodyType
{
    Null,
    Lake,
    River
}

public class Zone : MonoBehaviour
{
    // how does this adjusts itsekf for  water bodies 
    [SerializeField] public Activity zoneType;
    [SerializeField] private List<Vector3> zonePosition = new();
    [SerializeField] private SplineContainer splineGm;
    private Spline riverSpline => splineGm.Spline;
    private HashSet<Vector3> occupiedPositions = new();

    private int maxPosition = 6;
    private float disBtwPos = 5;

    private TextMeshProUGUI TypeText;
    private bool _isDrinkingZone;
    private int edgeOffset = 5;
    private int _attempts = 0;
    public bool HasAvailablePosition => zonePosition.Any(pos => !occupiedPositions.Contains(pos));
    private Dictionary<int, string> hourlyOccupancy = new(); // hour → species
    public WaterBody WaterBody { get; private set; }
    [SerializeField] private Transform circleCenter;
    [SerializeField] private float circleRadius = 10f;

// Arc to exclude (in degrees)
    [SerializeField] private float excludeStartAngle = 30;
    [SerializeField] private float excludeEndAngle;
    [SerializeField] private float excludeStartAngleB = 300f;
    [SerializeField] private float excludeEndAngleB = 340f;

// Offset for valid drinking positions
    [SerializeField] private float drinkOffset = 0f;
    [SerializeField] private int pointsCount = 20;

    private void Awake()
    {
        WaterBody = GetComponent<WaterBody>();
        _isDrinkingZone = WaterBody != null;
        GenerateZonePosition();
        TypeText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetZoneText(Activity zone)
    {
        TypeText.text = zone.ToString();
    }

    private void GenerateZonePosition()
    {
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
        List<float> angles = new();
        zonePosition.Clear();
        var position = circleCenter.position;

        for (var i = 0; i < pointsCount; i++)
        {
            var t = i / (float)(pointsCount - 1);
            var angle = t * 360f;

            angles.Add(angle);
            if (IsAngleInArc(angle, excludeStartAngle, excludeEndAngle))
            {
                continue;
            }

            if (IsAngleInArc(angle, excludeStartAngleB, excludeEndAngleB))
            {
                continue;
            }

            var rad = angle * Mathf.Deg2Rad;

            var pos = new Vector3(
                position.x + Mathf.Cos(rad) * (circleRadius + drinkOffset),
                position.y,
                position.z + Mathf.Sin(rad) * (circleRadius + drinkOffset)
            );
            Debug.DrawLine(position, pos, Color.red, 5f);
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 50, NavMesh.AllAreas))
            {
                zonePosition.Add(hit.position);
            }
        }

        print(zonePosition.Count);
    }

    private bool IsAngleInArc(float angle, float start, float end)
    {
        angle = NormalizeAngle(angle);
        start = NormalizeAngle(start);
        end = NormalizeAngle(end);

        // Normal arc (no wrap)
        if (start <= end)
            return angle >= start && angle <= end;

        // Wrapped arc (e.g., 300 → 40)
        return angle >= start || angle <= end;
    }

    float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a < 0f) a += 360f;
        return a;
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
        if (circleCenter != null)
        {
            //    Gizmos.color = Color.green;
            //  DrawCircle(circleCenter.position, circleRadius);

            Gizmos.color = Color.red;
            //  DrawArc(circleCenter.position, circleRadius, excludeStartAngle, excludeEndAngle);
            //    DrawArc(circleCenter.position, circleRadius, excludeStartAngleB, excludeEndAngleB);
            Gizmos.color = Color.blue;
            foreach (var pos in zonePosition)
            {
                Gizmos.DrawSphere(pos, 10);
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 100)
    {
        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 360f;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 point = new Vector3(
                center.x + Mathf.Cos(rad) * radius,
                center.y,
                center.z + Mathf.Sin(rad) * radius
            );

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }

    private void DrawArc(Vector3 center, float radius, float startAngle, float endAngle, int segments = 50)
    {
        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 point = new Vector3(
                center.x + Mathf.Cos(rad) * radius,
                center.y,
                center.z + Mathf.Sin(rad) * radius
            );

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }

    public bool IsAvailableForTime(string species, int startHour, int endHour)
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

    public void ReserveTime(string species, int startHour, int endHour)
    {
        for (int hour = startHour; hour < endHour; hour++)
        {
            hourlyOccupancy[hour] = species;
        }
    }

    private void GenerateRiverPos()
    {
        print("generate river pos");
        zonePosition.Clear();
        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);


            Vector3 pos = riverSpline.EvaluatePosition(t);
            Vector3 tan = riverSpline.EvaluateTangent(t);
            Vector3 tangent = tan.normalized;
            // Stable sideways direction
            Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;

            // Offset toward land

            Vector3 rightPos = pos + normal * drinkOffset + splineGm.transform.position;
            Vector3 leftPos = pos - normal * drinkOffset + splineGm.transform.position;
            var drinkPos = leftPos;
            if (NavMesh.SamplePosition(drinkPos, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            {
                zonePosition.Add(hit.position);
            }

            print(zonePosition.Count);
        }

        foreach (var pos in zonePosition)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = pos;
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