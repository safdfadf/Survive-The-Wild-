using System;
using UnityEngine;

namespace Player
{
   public class PlayerScentEmitter : MonoBehaviour
   {
      private WindSystem _windSystem;
      public ScentLevel scentLevel = ScentLevel.Low;
      public float baseScentStrength = 1f;

      [Header("Modifiers")] 
      public float movementIncreaseRate = 0.2f;
      public float injuryIncreaseRate = 0.5f;
      public float dirtyIncreaseRate = 0.1f;

      [Header("Decay")] public float naturalDecayRate = 0.05f;

      private float scentValue = 0f; // 0–1 internal value
      
      private void Awake()
      {
         _windSystem = FindAnyObjectByType<WindSystem>();
      }

      private void OnEnable()
      {
         EventBus.On5SecondsPassed += CheckScentLevel;
      }

      private void OnDisable()
      {
         EventBus.On5SecondsPassed -= CheckScentLevel;
      }
      private void CheckScentLevel()
      {
         // Natural decay
         scentValue -= naturalDecayRate * Time.deltaTime;
         scentValue = Mathf.Clamp01(scentValue);

         // Convert internal value → enum
         if (scentValue < 0.25f) scentLevel = ScentLevel.Low;
         else if (scentValue < 0.5f) scentLevel = ScentLevel.Average;
         else if (scentValue < 0.75f)
         {
            scentLevel = ScentLevel.High;
            //ui manager tell player
         }
         else
         {
            scentLevel = ScentLevel.VeryHigh;
            //uimanager tell player
         }
      }
      // Called by movement system, injury system, environment system
      public void AddScent(float amount)
      {
         scentValue = Mathf.Clamp01(scentValue + amount);
      }

      // Animals call this
      public float GetIntensityAt(Vector3 animalPos) 
      {
         Vector3 windDir = _windSystem.GetWindDirection();
         float windSpeed = _windSystem.GetWindSpeed();

         // Scent center pushed downwind
         Vector3 scentCenter = transform.position + windDir * windSpeed;

         float dist = Vector3.Distance(animalPos, scentCenter);

         // Inverse falloff
         float intensity = baseScentStrength / (dist * dist);

         // Scale by scent level
         switch (scentLevel)
         {
            case ScentLevel.Low: intensity *= 0.3f; break;
            case ScentLevel.Average: intensity *= 0.6f; break;
            case ScentLevel.High: intensity *= 1f; break;
            case ScentLevel.VeryHigh: intensity *= 1.5f; break;
         }
         return Mathf.Clamp01(intensity);
      }
   }
}