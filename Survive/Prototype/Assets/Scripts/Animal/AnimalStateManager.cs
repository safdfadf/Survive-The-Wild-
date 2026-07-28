using System;
using System.Collections.Generic;
using Animal.States;
using Player;
using UnityEngine;

public class AnimalStateManager:MonoBehaviour //ToDo : Change script name to player Detection System
{
   private List<AnimalData> _activeAnimalsData = new();
   private PlayerRepository _playerRepository;
   [Header("Scent Intensity Threshold")]
    public float calmThreshold = 0.05f;
    public float alertThreshold = 0.12f;
    private float alarmThreshold = .25f;
    
    [SerializeField] private float sprintAlertRange = 70f;
    [SerializeField] private float walkAlertRange   = 40f;

    [SerializeField] private float alertToAlarmTime = 2.0f;  // seconds of sustained noise to alarm
    [SerializeField] private float suspicionDecay   = 1.0f;

   private float baseIntensityThreshold = .01f;
   private AnimalHandler _animalHandler;
   public void AddActiveData(AnimalData activeAnimals)
   {
      _activeAnimalsData.Add(activeAnimals);
   }

   private void Awake()
   {
      _playerRepository = FindAnyObjectByType<PlayerRepository>();
      _animalHandler = GetComponent<AnimalHandler>();
   }

   private void Update()
   {
      CheckPlayerNoise();
   }

   private void OnEnable()
   {
   //   EventBus.OnWindChanged += CheckPlayerScent;
   //   EventBus.On5SecondsPassed += CheckPlayerNoise;
   }

   private void OnDisable()
   {
     // EventBus.OnWindChanged -= CheckPlayerScent;
     // EventBus.On5SecondsPassed -= CheckPlayerNoise;
   }
   //1) every time wind direction changes 
   //2) Noise system: every time player makes a sound more _ decimal animal in range will be able to hear it and change the state  

   private void CheckPlayerScent(Vector3 windDirection,float windSpeed)
   {
      for (int i = _activeAnimalsData.Count - 1; i >= 0; i--)
      {
         var data = _activeAnimalsData[i];
         Vector3? pos = data.CurrentPos;
         if (pos == null)return;
         if(_activeAnimalsData.Count <1){Debug.Log("active animal count zero");return;}
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
      Transform playerTransform = _playerRepository.GetPlayerTransform();
      Vector3 playerPos = playerTransform.transform.position;
      bool isSprinting  = _playerRepository.GetIsSprinting();
      bool isMoving     = _playerRepository.GetIsWalking();
      bool isCrouching  = _playerRepository.GetIsCrouching();

      Vector2 p = new Vector2(playerPos.x, playerPos.z);

      for (int i = _activeAnimalsData.Count - 1; i >= 0; i--)
      {
         AnimalData data = _activeAnimalsData[i];
         if (!data.CurrentPos.HasValue) continue;

         Vector2 a = new Vector2(data.CurrentPos.Value.x, data.CurrentPos.Value.z);
         float dist = Vector2.Distance(a, p);
        // Debug.Log(dist);

         bool shouldAlert = false;

         // ✅ Sprint rule
         if (isSprinting && dist <= sprintAlertRange)
         {
            shouldAlert = true;
         }
         // ✅ Walk rule
         else if (isMoving && !isCrouching && dist <= walkAlertRange)
         {
            shouldAlert = true;
         }
         // ✅ Crouch = safer (no forced alert)
         else
         {
            shouldAlert = false;
         }

         // Suspicion accumulation
         if (shouldAlert)
         {
            float fillRate = 1f / alertToAlarmTime;
            data.NoiseSuspicion += fillRate * Time.deltaTime;
         }
         else
         {
            data.NoiseSuspicion -= suspicionDecay * Time.deltaTime;
         }


         data.NoiseSuspicion = Mathf.Clamp01(data.NoiseSuspicion);

         // State logic
         if (data.NoiseSuspicion >= 1f)
         {
            ChangeToAlarmState(data);   // sustained too long
         }
         else if (data.NoiseSuspicion > 0f)
         {
            ChangeToAlertState(data);   // in zone or cooling down
         }
         else
         {
            ChangeToCalmState(data);
         }
      }
   }

   private void ChangeToCalmState(AnimalData data)
   {
     data.ChangeState(data.GetCalmState());
   }
   private void ChangeToAlertState(AnimalData data)
   {
      Debug.Log("ChangeToAlertState" +data.AnimalSo.prefab.name);
      data.ChangeState(data.GetAlertState());
   }

   private void ChangeToAlarmState(AnimalData data)
   {
      Debug.Log("ChangeToAlarmState");
     data.ChangeState(data.GetAlarmState());
     if(data.IsSpawned)return;
     _activeAnimalsData.Remove(data);
     _animalHandler.RemoveAnimalData(data);
   }
}
