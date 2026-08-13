using UnityEngine;

//ToDo : Remove this class 
public class WildBoar : ScheduledAnimal
{
    protected override void Awake()
    {
        myspecie = Species.Horse;

        base.Awake();
    }

    public override void Attack()
    {
        StartCoroutine(RamAttack());
    }
    // how do we choose which type of attack this animal will do 
}