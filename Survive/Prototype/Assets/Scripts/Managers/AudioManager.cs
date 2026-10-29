using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerSound> _playerAudioClips;

        private void OnEnable()
        {
            throw new NotImplementedException();
        }

        public void AnimalSound(AudioClip clip, float volume) // will be used by animal to trigger 
        {
        }

        public void PlayAttackSound()
        {
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