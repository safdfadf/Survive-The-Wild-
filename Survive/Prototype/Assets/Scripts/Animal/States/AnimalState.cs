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
        if (Clip == null)
        {
            Debug.Log("Clip was null");
            return;
        }
      
        var playerPos = PlayerRepository.instance.GetPlayerTransform().position;

        var distance = Vector3.Distance(playerPos, Animal.transform.position);

        if (distance > Radius)
        {
            Debug.Log("I don't have enough radius");
            return;
        }

        var volume = 1f - (distance / Radius);
        // what should be the distance 
        SceneRoot.Instance.AudioManager.AnimalSound(Clip, volume);
        Debug.Log(Clip.name +volume);
    }
}