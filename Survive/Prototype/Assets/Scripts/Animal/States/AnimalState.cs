using UnityEngine;

public abstract class AnimalState
{
    protected AnimalData data;
    protected AnimalBase Animal;
    public  AnimalState(AnimalData animalData)
    {
        data = animalData;
    }

    public AnimalState(){}
    public abstract void EnterState(AnimalBase animal);
    public abstract void UpdateState();
    public abstract void ExitState();
}
