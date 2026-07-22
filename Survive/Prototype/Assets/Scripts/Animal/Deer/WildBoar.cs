using UnityEngine;

public class WildBoar : AnimalBase
{
    protected override void Awake()
    {
        myspecie = Species.Horse;
        base.Awake();
    }
}
