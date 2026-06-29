using UnityEngine;

public class Horse : AnimalBase
{
    protected override void Awake()
    {
        myspecie = Species.Horse;
        base.Awake();
    }
}
