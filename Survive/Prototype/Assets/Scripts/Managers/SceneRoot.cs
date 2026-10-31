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
    public AudioManager AudioManager => audioManager;
    public EventManager EventManager => eventManager;
    public MiniGameHandler MiniGameHandler => miniGameHandler;

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