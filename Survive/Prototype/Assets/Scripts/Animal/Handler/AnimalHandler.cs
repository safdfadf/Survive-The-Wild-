using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using SurviveTheWild.Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class
    AnimalHandler : MonoBehaviour // this script will be responsible for creating new Animal data every time chunks in new regions are activated
{
    public GameObject UiMarker;
    public GameObject outOfBoundsZonePos;
    private List<AnimalSo> Sos;
    private readonly List<AnimalData> _activeData = new();
    private RegionType _currentRegion = RegionType.Null;
    private ScheduleManager _scheduleManager;
    private List<AnimalSo> _animalSo;
    private readonly Dictionary<AnimalData, GameObject> _soundUI = new();
    private AnimalStateManager _animalStateManager;
    private List<AnimalBase> _spawnedAnimals = new();
    public AnimalBase Attacker { get; set; }
    [SerializeField] private int Lod0;
    [SerializeField] private int Lod1;

    public void OnEnable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneHourPass += UpdateAnimalPos;
        EventBus.CreateAnimalData += CreateAnimalData; // Based on region create animal data
        SceneRoot.Instance.EventManager.ChunkEvents.ActiveChunk += CheckAnimalToSpawn;
        SceneRoot.Instance.EventManager.ChunkEvents.DeActiveChunk += CheckToDeactivate;
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += HandleLodOfActives;
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneHourPass -= UpdateAnimalPos;
        EventBus.CreateAnimalData -= CreateAnimalData;
        SceneRoot.Instance.EventManager.ChunkEvents.ActiveChunk -= CheckAnimalToSpawn;
        SceneRoot.Instance.EventManager.ChunkEvents.DeActiveChunk -= CheckToDeactivate;
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= HandleLodOfActives;
    }

    private void Awake()
    {
        _animalStateManager = GetComponent<AnimalStateManager>();
        _scheduleManager = new ScheduleManager();
    }

    private void Start()
    {
        _animalSo = SoProvider.instance.GetAnimalSo();
        _scheduleManager.GenerateSchedule(_animalSo);
    }

    private void CreateAnimalData(RegionType regionType, Bounds regionBounds)
    {
        if (Sos == null)
        {
            Sos = SoProvider.instance.GetAnimalSo();
        }

        if (_currentRegion == RegionType.Null)
        {
            _currentRegion = regionType;
            GenerateAnimal(_currentRegion, regionBounds);
        }
        // new Region Entered 
        else if (_currentRegion != regionType)
        {
            Debug.Log("new region");
            _activeData.Clear();
            _currentRegion = regionType;
            GenerateAnimal(_currentRegion, regionBounds);
        }
    }

    private void GenerateAnimal(RegionType regionType, Bounds regionBounds)
    {
        foreach (var so in Sos)
        {
            if (so.regionType == regionType)
            {
                if (!so.isScheduled) continue; // is its a non scheculed animal no need for data 

                int num = Random.Range(so.maxAmount, so.minAmount);
                for (int i = 0; i < num; i++)
                {
                    if (so == null)
                    {
                        Debug.Log(_activeData.Count);
                        return;
                    }

                    AnimalData data = new AnimalData(so, regionBounds, _scheduleManager, this);
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

    private void
        UpdateAnimalPos(int hour)
    {
        foreach (var data in _activeData)
        {
            data.OnHourChanged(hour);
            if (!data.CurrentPos.HasValue)
                continue;
        }
    }

    private void CheckAnimalToSpawn(Chunk chunk)
    {
        foreach (var data in _activeData)
        {
            if (data.CurrentPos.HasValue &&
                ChunkHelper.IsInChunkXZ(data.CurrentPos.Value, chunk.bounds))
            {
                ActivateAnimal(data);
            }
        }

        if (ChunkManager.Instance.IsPlayerInChunk(chunk))
            SpawnUnScheduledAnimal(chunk); // only spawn animals in which player is currently present
    }

    private void SpawnUnScheduledAnimal(Chunk chunk)
    {
        foreach (var animalSo in Sos)
        {
            if (Random.value <= animalSo.spawnProbability && !animalSo.isScheduled)
            {
                PosInChunk pos = chunk.cashedPos.FirstOrDefault(p => p.IsAvailable);

                if (pos == null)
                    continue;

                pos.IsAvailable = false;

                GameObject obj = GlobalPool.instance.Get(animalSo.prefab, pos.Position);


                UnScheduleAnimal unScheduleAnimal = obj.GetComponent<UnScheduleAnimal>();
                unScheduleAnimal.Initialize(animalSo, chunk.bounds);
                _spawnedAnimals.Add(unScheduleAnimal);
                chunk.objectInChunk.Add(obj); // recheck this 
            }
        }
    }

    private void ActivateAnimal(AnimalData data) // activates the animal and initializes it 
    {
        foreach (var So in _animalSo)
        {
            if (data.IsSpawned) continue;
            if (So == data.AnimalSo)
            {
                var obj = GlobalPool.instance.Get(So.prefab, data.CurrentPos.Value);
                data.AnimalInstance = obj;
                data.IsSpawned = true;
                ScheduledAnimal scheduledAnimal = obj.GetComponent<ScheduledAnimal>();
                if (scheduledAnimal == null)
                {
                    print(obj.name + " scheduledAnimal is null");
                    return;
                }

                scheduledAnimal.InitializeByData(data);
                scheduledAnimal.AnimalWrap(data.CurrentPos.Value);
                _spawnedAnimals.Add(scheduledAnimal);
            }
        }

        ActivateSound(data, _soundUI[data]);
    }

    private void ActivateSound(AnimalData data, GameObject obj)
    {
        if (obj.TryGetComponent<AnimalUi>(out var animalUi))
        {
            UIManager.instance.SetAnimalUI(obj);
            animalUi.Initialize(data);
        }
    }

    private void DeactivateAnimalUI(AnimalData data)
    {
        GlobalPool.instance.Return(UiMarker, data.AnimalUI);
    }

    private void CheckToDeactivate(Chunk chunk)
    {
        foreach (var animal in _spawnedAnimals)
        {
            if (animal.TryGetComponent<ScheduledAnimal>(out var scheduledAnimal))
            {
                var pos = scheduledAnimal.transform.position;
                if (chunk.bounds.Contains(pos))
                {
                    DeactivateAnimal(scheduledAnimal.AnimalData);
                }
            }
            else
            {
                if (chunk.bounds.Contains(animal.transform.position))
                {
                    GlobalPool.instance.Return(animal.AnimalSo.prefab, animal.gameObject);
                }
            }
        }

        foreach (var data in _activeData)
        {
            if (!data.IsSpawned) return;
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
        GlobalPool.instance.Return(data.AnimalSo.prefab, data.AnimalInstance);
        DeactivateAnimalUI(data);
    }

    public void HerdWarning(AnimalBase animal, Zone zone)
    {
        if (!_spawnedAnimals.Contains(animal)) return;
        foreach (var a in _spawnedAnimals)
        {
            if (a.AnimalSo.id != animal.AnimalSo.id || a == animal) continue; // same id check 
            var scheduledAnimal = a.GetComponent<ScheduledAnimal>();
            if (scheduledAnimal == null || scheduledAnimal.AnimalData.GetCurrentZone() != zone)
                return; // same zone check
            a.IsAggresive = false;
            var state = scheduledAnimal.AnimalData.GetAlarmState();
            scheduledAnimal.AnimalData.ChangeState(state);
        }
    }

    private void HandleLodOfActives()
    {
        if (_spawnedAnimals.Count == 0) return;
        foreach (var a in _spawnedAnimals)
        {
            var playerPos = PlayerRepository.instance.GetPlayerTransform().position;
            var AnimalPos = a.transform.position;
            float distance = Vector3.Distance(playerPos, AnimalPos);
            var level = GetLOD(distance);
            SceneRoot.Instance.EventManager.AnimalEvents.onUpdateLodLevel(level);
        }
    }

    private LodLevel GetLOD(float distance)
    {
        if (distance < 25) return LodLevel.Lod0;
        if (distance < 50) return LodLevel.Lod1;
        if (distance < 100) return LodLevel.Lod2;

        return LodLevel.Lod2;
    }

    public void RemoveAnimalData(AnimalData data)
    {
        if (!_activeData.Contains(data)) return;
        _activeData.Remove(data);
    }
}

public enum LodLevel
{
    Lod0,
    Lod1,
    Lod2
}