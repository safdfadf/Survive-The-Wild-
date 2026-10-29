using System;
using DefaultNamespace.EventBus;
using DefaultNamespace.Managers;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static SceneRoot Instance;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EventManager eventManager;
    public AudioManager AudioManager => audioManager;
    public EventManager EventManager => eventManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}