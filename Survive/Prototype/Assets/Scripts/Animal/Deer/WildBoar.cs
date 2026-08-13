using UnityEngine;

//ToDo : Remove this class 
public class WildBoar : ScheduledAnimal
{
    protected override void Awake()
    {
        myspecie = Species.Horse;

        base.Awake();
    }
}