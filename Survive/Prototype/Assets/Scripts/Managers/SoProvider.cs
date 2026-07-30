using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoProvider : MonoBehaviour // fix duplicate code in this script 
{
 public static SoProvider instance;

 [Header("Sos")] [SerializeField] private List<EnvironSo> environSo = new();
 [SerializeField] private List<ResourceSo> resourceSo = new();
 [SerializeField] private List<AnimalSo> animalSos = new();
 [SerializeField] private List<FoodSo> foodSo = new();

 [SerializeField] private GameObject Tracks;
 [SerializeField] private int itialTracksCount;

 private void Awake()
 {
  if (instance == null)
  {
   instance = this;
  }
  else
  {
   Destroy(gameObject);
  }

  InitializePool();
 }

 private void InitializePool() // pool creates and stores prefab required in one chunk 
 {
  foreach (var so in environSo)
  {
   GlobalPool.instance.PreWarm(so.prefab, so.amount);
  }

  foreach (var so in resourceSo)
  {
   GlobalPool.instance.PreWarm(so.prefab, so.amount);
  }

  GlobalPool.instance.PreWarm(Tracks, itialTracksCount);
 }

 public List<EnvironSo>
  GetEnvironmentSo(RegionType type) // this fuction can be a right place check if this chunk is in water bodies 
 {
  List<EnvironSo> result = new();
  foreach (var So in environSo)
  {
   if (So.regionType == type)
   {
    result.Add(So);
   }
  }

  return result;
 }

 private bool Probability(ISpawnedItem so)
 {
  // chance = 0–1 (e.g., 0.25 = 25%)
  return Random.value <= so.SpawningProbability;
 }

 public List<FoodSo> GetFoodSo(RegionType type) // sends food so list to be spawned 
 {

  List<FoodSo> result = new();
  foreach (var so in foodSo)
  {
   if (so.regionType == type && Probability(so))
   {
    result.Add(so);
   }
  }

  return result;
 }

 public List<ResourceSo> GetResourceSo()
 {
  List<ResourceSo> result = new();
  // are reources region based, to some extent yes and region like 
  foreach (var so in resourceSo)
  {
   result.Add(so);
  }

  return result;
 }

 public List<AnimalSo> GetAnimalSo()
 {
  return animalSos;
 }

 public GameObject GetTrack()
 {
  return Tracks;
 }

 public List<FoodSo> GetFoodSo()
 {
  return foodSo;
 }

 public ResourceSo GetSoForPrefab(GameObject prefab)
 {
  foreach (var so in resourceSo)
  {
   if (so.prefab == prefab)
   {
    return so;
   }
  }
  return null;
 }

public EnvironSo GetTreeSo()
 {
  
   foreach (var so in environSo)
   {
    if (so.name == "Tree")
     return so;
   }
   return null;
 }
}
