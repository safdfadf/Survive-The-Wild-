using UnityEngine;

public enum Activity
{
    Resting,
    Feeding,
    Drinking
}

public enum  Species
{
   Deer,Antelope,Elephant,Buffalo,Horse
}

[System.Serializable]
public class SpeciesDate// will be used by region 
{
    public Species specie;
    public int count;
}
[System.Serializable]
public class Schedule
{
    public Species species;
    public int startHour;
    public int endHour;
    public Activity zoneType;
    public Zone assignedZone;
}
