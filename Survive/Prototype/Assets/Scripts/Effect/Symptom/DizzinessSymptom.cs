using Effect.Symptoms;
using UnityEngine;

public class DizzinessSymptom : Symptom
{
    public DizzinessSymptom(PlayerBody player) : base(player)
    {
    }

    public override void Apply()
    {
    }

    public override void StartSymptom()
    {
        // blurry camera 
        // audio effect
    }

    public override void UpdateSymptom()
    {
        throw new System.NotImplementedException();
    }

    public override void StopSymptom()
    {
        throw new System.NotImplementedException();
    }
}