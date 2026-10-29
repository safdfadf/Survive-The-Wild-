using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace DefaultNamespace.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerSound> _playerAudioClips;
        private AudioSource _audioSource;
        private Queue<AnimalAudioRequest> animalQueue = new();
        private float nextPlayTime;
        [SerializeField] private float cooldown = 4f;
        private bool canPlay;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void AnimalSound(AudioClip clip,
            float volume)
        {
            animalQueue.Enqueue(
                new AnimalAudioRequest()
                {
                    Clip = clip,
                    Volume = volume
                });
            StopCoroutine(HandleAnimalSound());
        }

        private IEnumerator HandleAnimalSound()
        {
            if (animalQueue.Count == 0) yield break;
            {
                var request =
                    animalQueue.Dequeue();

                _audioSource.PlayOneShot(
                    request.Clip,
                    request.Volume);

                yield return new WaitForSeconds(cooldown);
            }
        }

        public void PlayerSound(SoundType soundType)
        {
        }
    }
}

public enum SoundType
{
    Hurt,
    Winded,
    Attack,
    Drinking,
    Food,
    WashSelf
}

public class PlayerSound
{
    public AudioClip Clip;
    public SoundType Type;
}

public class AnimalAudioRequest
{
    public AudioClip Clip;
    public float Volume;
}