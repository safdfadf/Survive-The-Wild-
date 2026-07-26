using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AnimalHandler : MonoBehaviour// this script will be responsible for creating new Animal data every time chunks in new regions are activated
{
    public GameObject UiMarker;
    public GameObject outOfBoundsZonePos;
    private List<AnimalSo> Sos;
    private readonly List<AnimalData> _activeData= new();
    private RegionType _currentRegion= RegionType.Null;
    private ScheduleManager _scheduleManager;
    private List<AnimalSo> _animalSo;
    private readonly Dictionary<AnimalData, GameObject> _soundUI = new();
    private AnimalStateManager _animalStateManager;
    // for every animal data, when player is close or both animal and player is in same active chunk start the ui 

    public void OnEnable()
    {
        EventBus.OnHourChanged += UpdateAnimalPos; // update animal current position
        EventBus.CreateAnimalData += CreateAnimalData; // Based on region create animal data
        EventBus.OnChunkChanged += CheckAnimalToSpawn; // check if any animal lies in chunk bounds
        EventBus.OnDeactiveChunk += CheckToDeactivate;// Deactivate animal
    }

    private void OnDisable()
    {
        EventBus.OnHourChanged -= UpdateAnimalPos;
        EventBus.CreateAnimalData -= CreateAnimalData;
        EventBus.OnChunkChanged -= CheckAnimalToSpawn;
        EventBus.OnDeactiveChunk -= CheckToDeactivate;
    }

    private void Awake()
    {
        _animalStateManager = GetComponent<AnimalStateManager>();
        _scheduleManager = new ScheduleManager();
        _scheduleManager.GenerateSchedule();
    }
    private void Start()
    {
        _animalSo = SoProvider.instance.GetAnimalSo();
    }
    private void CreateAnimalData(RegionType regionType,Bounds regionBounds)
    {
        if (Sos == null)
        {
            Sos = SoProvider.instance.GetAnimalSo();
        }
        if (_currentRegion ==  RegionType.Null)
        {
            _currentRegion = regionType;
            GenerateAnimal(_currentRegion,regionBounds);
        }
        // new Region Entered 
        else if (_currentRegion != regionType)
        {
            Debug.Log("new region");
            _activeData.Clear();
            _currentRegion = regionType;
            GenerateAnimal(_currentRegion,regionBounds);
        }
    }

    private void GenerateAnimal(RegionType regionType,Bounds regionBounds)
    {
        foreach (var so in Sos)
        {
            if (so.regionType == regionType)
            {
                if(!so.isScheduled)continue;// is its a non scheculed animal no need for data 
                
                int num = Random.Range(so.maxAmount, so.minAmount);
                for (int i = 0; i < num; i++)
                {
                    if(so == null){ Debug.Log(_activeData.Count);return;}
                    AnimalData data = new AnimalData(so,regionBounds,_scheduleManager,this);
                    _activeData.Add(data);
                    _animalStateManager.AddActiveData(data);
                    data.AnimalSo = so;

                    if (data.CurrentPos.HasValue)
                    {
                        GameObject marker = GlobalPool.instance.Get(UiMarker, Vector3.zero);
                        marker.SetActive(false);
                       data.AnimalUI = marker;
                        _soundUI[data] = marker;
                    }

                }
            }
        }
    }
    private void UpdateAnimalPos(int hour)// every hour check if position of the animal needs to be changed based on schedule
    {
        foreach (var data in _activeData)
        {
            data.OnHourChanged(hour);
            if (!data.CurrentPos.HasValue)
                continue;
        }
    }
    private void CheckAnimalToSpawn(Chunk chunk)// if animal lies in active chunk spawn it   
    {
         foreach (var data in _activeData) 
         {
           if(!data.CurrentPos.HasValue)Debug.Log("currentPOs issue");
            if (data.CurrentPos.HasValue && chunk.bounds.Contains(data.CurrentPos.Value))
            {
                ActivateAnimal(data);
            }
         }
         // check probability in chunk for non scheduled animals 
         
    }
    private void ActivateAnimal(AnimalData data) // activates the animal and initializes it 
    {
        foreach (var So in _animalSo)
        {
            if(data.IsSpawned)continue;
            if (So.specie == data.Specie)
            {
                Debug.Log("Activate animal");
               GameObject obj= GlobalPool.instance.Get(So.prefab, data.CurrentPos.Value);
               data.AnimalInstance = obj;
                data.IsSpawned = true;
            //    data.AnimalSo = So;
                ScheduledAnimal scheduledAnimal = obj.GetComponent<ScheduledAnimal>();
                scheduledAnimal.InitializeByData(data);
                scheduledAnimal.AnimalWrap(data.CurrentPos.Value);
            }
        }
        ActivateSound(data,_soundUI[data]);
    }
    private void ActivateSound(AnimalData data,GameObject obj)
    {
        if (obj.TryGetComponent<AnimalUi>(out var animalUi))
        {
           
            UIManager.instance.SetAnimalUI(obj);
            animalUi.Initialize(data);
        }
    }
    private void DeactivateAnimalUI(AnimalData data)
    {
      GlobalPool.instance.Return(UiMarker,data.AnimalUI);
    }
    private void CheckToDeactivate(Chunk chunk)
    {
        foreach (var data in _activeData)
        {
            if(!data.IsSpawned)return;
            Vector3 pos = data.AnimalInstance.transform.position;
            if (chunk.bounds.Contains(pos))
            {
                DeactivateAnimal(data);
            }
        }
    }
    public void DeactivateAnimal(AnimalData data)
    {
      Debug.Log("Deactivate animal");
        data.IsSpawned = false;
        GlobalPool.instance.Return(data.AnimalSo.prefab,data.AnimalInstance);
        DeactivateAnimalUI(data);
    }

    public void RemoveAnimalData(AnimalData data)
    {
        if(!_activeData.Contains(data))return;
        _activeData.Remove(data);
    }
}
