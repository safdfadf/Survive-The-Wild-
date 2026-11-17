using System;
using SurviveTheWild.EventBus;
using SurviveTheWild.Managers;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public static SceneRoot Instance;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EventManager eventManager;
    [SerializeField] public MiniGameHandler miniGameHandler;
    [SerializeField] private TimeManager timeManager;
    public AudioManager AudioManager => audioManager;
    public EventManager EventManager => eventManager;
    public MiniGameHandler MiniGameHandler => miniGameHandler;
    public TimeManager TimeManager => timeManager;

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