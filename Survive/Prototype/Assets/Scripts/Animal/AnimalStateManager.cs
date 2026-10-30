using System;
using System.Collections.Generic;
using Animal.States;
using DefaultNamespace.EventBus;
using DefaultNamespace.Helpers;
using Player;
using UnityEngine;

public class AnimalStateManager : MonoBehaviour //ToDo : Change script name to player Detection System
{
    private List<AnimalData> _activeAnimalsData = new();
    private PlayerRepository _playerRepository;
    [Header("Scent Intensity Threshold")] public float calmThreshold = 0.05f;
    public float alertThreshold = 0.12f;
    private float alarmThreshold = .25f;

    [SerializeField] private float sprintAlertRange = 70f;
    [SerializeField] private float walkAlertRange = 40f;

    [SerializeField] private float alertToAlarmTime = 2.0f; // seconds of sustained noise to alarm
    [SerializeField] private float suspicionDecay = 1.0f;

    private float baseIntensityThreshold = .01f;
    private AnimalHandler _animalHandler;
    private AnimalState _currentState;
    private Dictionary<int, List<AnimalData>> _animalData = new();
    private int index = 0;
    private Bounds _currentChunkBounds;
    private AnimalData _currentAnimal;

    public void AddActiveData(AnimalData activeAnimal)
    {
        foreach (var kvp in _animalData)
        {
            List<AnimalData> group = kvp.Value;

            if (group.Count == 0) continue;

            if (group[0].GetCurrentZone() == activeAnimal.GetCurrentZone())
            {
                group.Add(activeAnimal);
                return;
            }
        }

        _animalData.Add(index, new List<AnimalData> { activeAnimal }); // it basically stores animal in the same zone 
        index++;
    }


    private void Awake()
    {
        _playerRepository = FindAnyObjectByType<PlayerRepository>();
        _animalHandler = GetComponent<AnimalHandler>();
    }

    private void OnEnable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += CheckPlayerNoise;
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= CheckPlayerNoise;
    }

    private void CheckPlayerScent(Vector3 windDirection, float windSpeed)
    {
        for (int i = _activeAnimalsData.Count - 1; i >= 0; i--)
        {
            var data = _activeAnimalsData[i];
            Vector3? pos = data.CurrentPos;
            if (pos == null) return;
            if (_activeAnimalsData.Count < 1)
            {
                Debug.Log("active animal count zero");
                return;
            }

            float intensity = _playerRepository.GetScentIntensity(pos.Value);


            if (intensity > alarmThreshold)
            {
                ChangeToAlarmState(data);
            }
            else if (intensity >= baseIntensityThreshold)
            {
                ChangeToAlertState(data);
            }
            else
            {
                ChangeToCalmState(data);
            }
        }
    }


    private void CheckPlayerNoise()
    {
        Bounds bounds = ChunkManager.Instance.CurrentBounds;
        var playerTransform = _playerRepository.GetPlayerTransform();
        var playerPos = playerTransform.position;

        var isSprinting = _playerRepository.GetIsSprinting();
        var isMoving = _playerRepository.GetIsWalking();
        var isCrouching = _playerRepository.GetIsCrouching();

        Vector2 p = new Vector2(playerPos.x, playerPos.z);

        foreach (var kvp in _animalData)
        {
            List<AnimalData> group = kvp.Value;
            if (group.Count == 0) continue;

            AnimalData first = group[0];
            if (!first.CurrentPos.HasValue) continue;

            var firstPos = first.CurrentPos.Value;

            if (!ChunkHelper.IsInChunkXZ(firstPos, bounds))
            {
//                print("conyds");
                continue;
            }

            Vector3 animalPos3D = first.CurrentPos.Value;
            Vector2 a = new Vector2(animalPos3D.x, animalPos3D.z);

            float dist = Vector2.Distance(a, p);

            bool shouldAlert = false;

            if (isSprinting && dist <= sprintAlertRange)
                shouldAlert = true;
            else if (isMoving && !isCrouching && dist <= walkAlertRange)
                shouldAlert = true;

            if (shouldAlert)
            {
                float fillRate = 1f / alertToAlarmTime;
                first.NoiseSuspicion += fillRate * 1;
            }
            else
            {
                first.NoiseSuspicion -= suspicionDecay * 1;
            }

            first.NoiseSuspicion = Mathf.Clamp01(first.NoiseSuspicion);

            // State logic
            if (first.NoiseSuspicion >= 1f)
            {
                ChangeToAlarmState(first);
            }
            else if (first.NoiseSuspicion > 0f)
            {
                ChangeToAlertState(first);
            }
            else
            {
                ChangeToCalmState(first);
            }
        }
    }

    private void ChangeToCalmState(AnimalData data)
    {
        AnimalState newState = data.GetCalmState();
        if (_currentState != null && _currentState.Equals(newState)) return;
        _currentState = newState;
        data.isLeader = true;
        data.ChangeState(newState);
    }

    private void ChangeToAlertState(AnimalData data)
    {
        AnimalState newState = data.GetAlertState();
        if (_currentState != null && _currentState.Equals(newState)) return;
        _currentState = newState;
        data.ChangeState(newState);
    }

    private void ChangeToAlarmState(AnimalData data)
    {
        Debug.Log("Changing State");
        AnimalState newState = data.GetAlarmState();
        if (_currentState != null && _currentState.Equals(newState)) return;
        _currentState = newState;
        data.ChangeState(newState);
        //     if (data.IsSpawned) return;
        //    _activeAnimalsData.Remove(data);

        //  _animalHandler.RemoveAnimalData(data);
    }
}