using UnityEngine;

public abstract class AnimalState
{
    protected AnimalData data;

    public  AnimalState(AnimalData animalData)
    {
        data = animalData;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
