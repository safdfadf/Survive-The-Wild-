using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public GameObject zonePrefab;
    public static ZoneManager Instance;
    private Dictionary<Activity, List<Zone>> _zoneByType = new();

    private float _zoneRadius = 10f;
    private List<Zone> _drinkingZone = new();

    // here tell the chunk manager that we dont need to spawn environment here or spawn water body specific environment
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

        StoreDrinkingZone();
    }

    private void StoreDrinkingZone()
    {
        _drinkingZone = FindObjectsByType<Zone>(FindObjectsSortMode.InstanceID)
            .Where(z => z.waterBodyType != WaterBody.Null).ToList();
        if (!_zoneByType.ContainsKey(Activity.Drinking))
            _zoneByType[Activity.Drinking] = new List<Zone>();

        foreach (var zone in _drinkingZone)
        {
            _zoneByType[Activity.Drinking].Add(zone);
        }
    }

    private void CreateZone(Activity type, Vector3 position) // this function will creteate zones
    {
        var zoneGo = Instantiate(zonePrefab, position, Quaternion.identity);
        Zone zone = zoneGo.GetComponent<Zone>();
        zone.waterBodyType = WaterBody.Null;
        zone.zoneType = type;
        zone.SetZoneText(type);
        SphereCollider collider = zoneGo.GetComponent<SphereCollider>();
        collider.radius = _zoneRadius;
        if (!_zoneByType.ContainsKey(type))
            _zoneByType[type] = new List<Zone>(); // checks if there is a list by that key if not it creates the list
        _zoneByType[type].Add(zone); // added to the available zones  
    }

    public Zone GetAvailableZone(Activity type, Species species, int startHour, int endHour, Bounds regionBounds)
    {
        if (!_zoneByType.ContainsKey(type))
            _zoneByType[type] = new List<Zone>();

        foreach (var zone in _zoneByType[type])
        {
            if (regionBounds.Contains(zone.transform.position) &&
                zone.HasAvailablePosition &&
                zone.IsAvailableForTime(species, startHour, endHour))
            {
                zone.ReserveTime(species, startHour, endHour);
                return zone;
            }
        }

        // Create new zone inside region bounds
        Vector3 origin = regionBounds.center;

        origin.y = 0f;

        Vector3 newPos = RetPosOnNv.ReturnRandomNavMeshPos(regionBounds);
        //  if (!regionBounds.Contains(newPos)) {Debug.Log(" zone position is outside region bounds: {newPos}\"");return null;}

        CreateZone(type, newPos);
        Zone newZone = _zoneByType[type].Last();
        newZone.ReserveTime(species, startHour, endHour);
        return newZone;
    }

}