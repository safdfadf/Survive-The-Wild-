using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class ZoneManager : MonoBehaviour
{
    [Header("Lake Properties")] [SerializeField]
    private Transform circleCenter;

    [SerializeField] private float circleRadius = 10f;

    [SerializeField] private float circleOffset;
    [SerializeField] private float excludeStartAngle = 30;
    [SerializeField] private float excludeEndAngle;
    [SerializeField] private float excludeStartAngleB = 300f;
    [SerializeField] private float excludeEndAngleB = 340f;

    [FormerlySerializedAs("drinkOffset")] [SerializeField]
    private float riverOffset = 0f;

    [SerializeField] private int pointsCount = 20;

    [Header("River Properties")] [SerializeField]
    private SplineContainer[] splines;

    public GameObject zonePrefab;
    public static ZoneManager Instance;
    private Dictionary<Activity, List<Zone>> _zoneByType = new();

    private float _zoneRadius = 10f;
    private List<Zone> _drinkingZone = new();

    [SerializeField] float riverWidth = 8f; // total width of river
    [SerializeField] float riverHeight = 5f; // vertical height of bounds

    [SerializeField] float step = 0.05f; // spline sampling resolution


    private List<Bounds> _waterBounds = new();
    private int debugCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetEmptyZone();
    }

    private void SetEmptyZone()
    {
        RiverBodyBounds();
        LakeBodyBounds();
    }

    private Zone CreateZone(Activity type, Vector3 position) // this function will creteate zones
    {
        var zoneGo = Instantiate(zonePrefab, position, Quaternion.identity);
        Zone zone = zoneGo.GetComponent<Zone>();
        zone.zoneType = type;
        zone.SetZoneText(type);
        SphereCollider collider = zoneGo.GetComponent<SphereCollider>();
        collider.radius = _zoneRadius;
        if (!_zoneByType.ContainsKey(type))
            _zoneByType[type] = new List<Zone>(); // checks if there is a list by that key if not it creates the list
        _zoneByType[type].Add(zone); // added to the available zones  
        return zone;
    }

    public Zone GetAvailableZone(Activity type, string species, int startHour, int endHour, Bounds regionBounds)
    {
        if (!_zoneByType.ContainsKey(type))
            _zoneByType[type] = new List<Zone>();

        foreach (var zone in _zoneByType[type])
        {
            if
                (zone.HasAvailablePosition &&
                 zone.IsAvailableForTime(species, startHour, endHour))
            {
                zone.ReserveTime(species, startHour, endHour);
                return zone;
            }
            else
            {
                print(regionBounds.Contains(zone.transform.position));
            }
        }

        if (type == Activity.Drinking)
        {
            var r = Random.Range(0, 2);
            if (r == 0)
            {
                return CreateLakeZone();
            }
            else
            {
                return CreateRiverZone();
            }
        }

        Zone newZone = CreateMoreZones(regionBounds, type);
        newZone.ReserveTime(species, startHour, endHour);
        return newZone;
    }

    private Zone CreateMoreZones(Bounds regionBounds, Activity type)
    {
        var origin = regionBounds.center;

        origin.y = 0f;
        var newPos = RetPosOnNv.ReturnRandomNavMeshPos(regionBounds);
        bool posValid = false;
        if (type != Activity.Drinking) // if type is not drinking then male prosition is not on water body
            while (!posValid)
            {
                newPos = RetPosOnNv.ReturnRandomNavMeshPos(regionBounds);
                posValid = CheckIfPosLiesInWaterBody(newPos);
            }

        Vector3 flatPos = new Vector3(newPos.x, regionBounds.center.y, newPos.z); // specificlly check x and z

        if (!regionBounds.Contains(flatPos))
        {
            Debug.Log($"Zone position is outside region bounds: {newPos}");
            return null;
        }

        return CreateZone(type, newPos);
    }

    private bool CheckIfPosLiesInWaterBody(Vector3 position)
    {
        foreach (var bounds in _waterBounds)
        {
            if (bounds.Contains(position))
                return false;
        }

        return true;
    }

    private Zone CreateLakeZone() // creates one zone at a time
    {
        // depnding 
        while (true)
        {
            var position = circleCenter.position;
            float angle = 0;
            var canPassThrough = false;
            while (!canPassThrough)
            {
                float value = Random.Range(0, 360);
                var t = value / (float)(pointsCount - 1);
                angle = t * 360f;
                if (!IsAngleInArc(angle, excludeStartAngle, excludeEndAngle) &&
                    !IsAngleInArc(angle, excludeStartAngleB, excludeEndAngleB)) canPassThrough = true;
            }

            var rad = angle * Mathf.Deg2Rad;

            var pos = new Vector3(position.x + Mathf.Cos(rad) * (circleRadius + circleOffset), position.y,
                position.z + Mathf.Sin(rad) * (circleRadius + circleOffset));
            Debug.DrawLine(position, pos, Color.red, 5f);
            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 50, NavMesh.AllAreas))
            {
                print("hit");
                return CreateZone(Activity.Drinking, hit.position);
                // take radius of the circle and create bounds and ask chunk manager 
            }
        }
    }

    private Zone CreateRiverZone()
    {
        print("Creating river zone");
        float totalRivers = splines.Length;
        SplineContainer chosenSpline = splines[(int)Random.Range(0, totalRivers)];
        // now since we have 
        var t = Random.Range(0, 20) / (float)(pointsCount - 1);


        Vector3 pos = chosenSpline.Spline.EvaluatePosition(t);
        Vector3 tan = chosenSpline.Spline.EvaluateTangent(t);
        var tangent = tan.normalized;
        // Stable sideways direction
        Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;

        // Offset toward land

        var rightPos = pos + normal * riverOffset + chosenSpline.gameObject.transform.position;
        Vector3 leftPos = pos - normal * riverOffset + chosenSpline.gameObject.transform.position;
        var drinkPos = leftPos;
        if (NavMesh.SamplePosition(drinkPos, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            return CreateZone(Activity.Drinking, hit.position);
        }

        return null;
    }

    private void RiverBodyBounds()
    {
        foreach (var Sc in splines)
        {
            var spline = Sc.Spline;

            for (float t = 0; t < 1f; t += step)
            {
                Vector3 p0 = (Vector3)spline.EvaluatePosition(t) + Sc.transform.position;
                Vector3 p1 = (Vector3)spline.EvaluatePosition(t + step) + Sc.transform.position;

                Vector3 mid = (p0 + p1) * 0.5f;

                Vector3 tangent = (p1 - p0).normalized;

                Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;

                Vector3 size = new Vector3(
                    riverWidth, // width across river
                    riverHeight, // height
                    (p1 - p0).magnitude // length of this segment
                );

                Bounds segmentBounds = new Bounds(mid, size);
                _waterBounds.Add(segmentBounds);
                ChunkManager.Instance.SetEmptyChunk(segmentBounds);
            }
        }
    }

    private void LakeBodyBounds()
    {
        Bounds lakeBounds = new Bounds(circleCenter.position,
            new Vector3(circleRadius, 1, circleRadius));
        _waterBounds.Add(lakeBounds);
        ChunkManager.Instance.SetEmptyChunk(lakeBounds);
    }

    private bool IsAngleInArc(float angle, float start, float end)
    {
        angle = NormalizeAngle(angle);
        start = NormalizeAngle(start);
        end = NormalizeAngle(end);

        if (start <= end)
            return angle >= start && angle <= end;

        return angle >= start || angle <= end;
    }

    float NormalizeAngle(float a)
    {
        a %= 360f;
        if (a < 0f) a += 360f;
        return a;
    }
}