using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerSound> _playerAudioClips = new();
        [SerializeField] private float volumeConstant;
        [SerializeField] private float cooldown = 4f;
        private AudioSource _audioSource;
        private Queue<AnimalAudioRequest> animalQueue = new();
        private float nextPlayTime;
        private bool canPlay;

        private float volume;

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
            StartCoroutine(HandleAnimalSound());
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
            PlayerSound sound = _playerAudioClips.Find(x => x.Type == soundType);

            if (sound == null || sound.Clips.Count == 0)
                return;

            AudioClip randomClip = sound.Clips[UnityEngine.Random.Range(0, sound.Clips.Count)];

            _audioSource.PlayOneShot(randomClip, volumeConstant);
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
    Vomit,
    WashSelf,
    Tired,
    MorningYawn
}

[System.Serializable]
public class PlayerSound
{
    [FormerlySerializedAs("Clip")] public List<AudioClip> Clips;
    public SoundType Type;
}

public class AnimalAudioRequest
{
    public AudioClip Clip;
    public float Volume;
}