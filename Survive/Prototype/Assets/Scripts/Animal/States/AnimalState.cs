using Player;
using UnityEngine;

public abstract class AnimalState
{
    protected AnimalData data;
    protected AnimalBase Animal;

    public AnimalState(AnimalData animalData)
    {
        data = animalData;
    }

    public AnimalState()
    {
    }

    public abstract void EnterState(AnimalBase animal);
    public abstract void UpdateState();
    public abstract void ExitState();

    protected virtual void HandleAudioCall(int Radius, AudioClip Clip)
    {
        var playerPos = PlayerRepository.instance.GetPlayerTransform().position;

        var distance = Vector3.Distance(playerPos, Animal.transform.position);

        if (distance > Radius)
            return;

        var volume = 1f - (distance / Radius);

        SceneRoot.Instance.AudioManager.AnimalSound(Clip, volume);
    }
}