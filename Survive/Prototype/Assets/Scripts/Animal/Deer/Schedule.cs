using UnityEngine;

public enum Activity
{
    Resting,
    Feeding,
    Drinking
}

public enum Species // Todo: Use ids instead 
{
    Deer,
    Antelope,
    Elephant,
    Buffalo,
    Horse
}

[System.Serializable]
public class Schedule
{
    public string specie;
    public int startHour;
    public int endHour;
    public Activity zoneType;
    public Zone assignedZone;
}

[System.Serializable]
public class AttackSchedule
{
    public int Day;
    public int Hour;

    public GameObject PredatorPrefab;

    public bool Triggered;
}
