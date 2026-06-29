using System.Diagnostics;
using Animal.States;
using UnityEngine;

public class TrackData
{
    public Vector3 pos;
    public float Angle;
    public Species species;
    public float TimeStamp; //age
    public bool isInjured;
    public int maxTrackAge;
    public string Dir;
    public TrackAge trackAge;
    public AnimalState AnimalState;
    public AnimalSo soAnimal;
    public GameObject prefab;

    public TrackData(AnimalSo so,Vector3 pos, Species species, float timeStamp, bool isInjured, int maxTrackAge,string Direction,float angle,AnimalState state)
    {
        this.pos = pos;
        this.species = species;
        TimeStamp = timeStamp;
        this.isInjured = isInjured;
        this.maxTrackAge = maxTrackAge;
        Dir = Direction;
        AnimalState = state;
        soAnimal = so;
        Angle = angle;
    }
    public TrackData()
    {
        
    }

    public void UpdateTrackAge()
    {
        switch (TimeStamp)
        {
         case  < 1 :
             trackAge = TrackAge.VeryFresh; 
            break;
         case < 6:
             trackAge = TrackAge.Fresh;
             break;
         case < 12:
             trackAge = TrackAge.Recent;
             break;
         case < 20:
             trackAge = TrackAge.Old;   
             break;
        }
    }
}
