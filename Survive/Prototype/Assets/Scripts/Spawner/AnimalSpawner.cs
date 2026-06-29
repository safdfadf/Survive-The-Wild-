using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour // dynamic animal spawner, which will be actively spawning and removing animal based on player location  
{
   [SerializeField] private List<AnimalSo> animalSo;
   private Dictionary<Species, GameObject> _animalsPool =new();
   private Dictionary<Species,List<Schedule>>  _activeSchedules = new();
   private ScheduleManager _scheduleManager;
   private RegionType _currentRegion;
   private void Awake()
   {
    
      foreach (var animal in animalSo)
      {
         _animalsPool[animal.specie] = animal.prefab;
      }
   }
   

   private IEnumerator SpawnLoop(Species species, int count )
   {
      while (true)
      {
         int currentHour = TimeManager.Instance.GetCurrentHour();

         foreach (var entry in _activeSchedules[species])
         {
            if (IsHourInRange(currentHour, entry.startHour, entry.endHour))
            {
               for (int i = 0; i < count; i++)
               {
                  SpawnAnimal(entry);
               }
            }
         }

         yield return new WaitForSeconds(60f); // simulate 1 in-game hour
      }
   }

   private void SpawnAnimal(Schedule entry)
   { 
      Debug.Log("Spawning animal: " + entry.species);
      if (entry.assignedZone == null) {Debug.Log("assigned zone is "+entry.assignedZone );return;}
      Zone zone = entry.assignedZone;
      Vector3 pos = zone.RequestPosition() ??Vector3.zero;
      GameObject prefab = _animalsPool[entry.species];
      GameObject animal = Instantiate(prefab, pos, Quaternion.identity);

      AnimalBase animalScript = animal.GetComponent<AnimalBase>();
     // animalScript.Initialize(entry.species,_activeSchedules[entry.species],zone,pos);
   }
   bool IsHourInRange(int currentHour, int startHour, int endHour)
   {
      if (startHour <= endHour)
      {
         // Normal range (e.g. 8–16)
         return currentHour >= startHour && currentHour <= endHour;
      }
      else
      {
         // Overnight range (e.g. 22–2)
         return currentHour >= startHour || currentHour <= endHour;
      }
   }

}


