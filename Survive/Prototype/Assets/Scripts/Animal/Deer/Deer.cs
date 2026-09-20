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
}