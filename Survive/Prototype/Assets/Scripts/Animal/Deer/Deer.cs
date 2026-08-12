using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Deer : ScheduledAnimal //ToDo:  remove script
{
    protected override void Awake()
    {
        myspecie = Species.Deer;
        base.Awake();
    }

    private void Start()
    {
        if (animator == null)
        {
            Debug.Log("animator is null");
        }
    }

    // ToDo: remove this override function 
    public override void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null)
    {
        if (CurrentState == CalmState && !AnimalData.isZoneTraveling)
        {
            speedOverride = walkSpeed;
        }
        else if (CurrentState == AlertState && !AnimalData.isZoneTraveling)
        {
            speedOverride = alertSpeed;
        }
        else if (CurrentState == AlertState)
        {
            speedOverride = runSpeed;
        }

        base.MoveTo(destination, onArrived, speedOverride);
    }
}