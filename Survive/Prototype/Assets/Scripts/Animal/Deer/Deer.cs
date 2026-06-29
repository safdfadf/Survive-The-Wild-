using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Deer : AnimalBase
{
  
    protected override void Awake()
    {
        // get all the objets with hit b
        
        myspecie = Species.Deer;
        base.Awake();
       
    }
    private  void Start()
    {
        if (animator == null)
        {
            Debug.Log("animator is null");
        }
    }

    public override void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null)
    {
      
        if (CurrentState == CalmState && !AnimalData.isZoneTraveling)
        {
            speedOverride = walkSpeed;
        }
        else if(CurrentState == AlertState&& !AnimalData.isZoneTraveling)
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
