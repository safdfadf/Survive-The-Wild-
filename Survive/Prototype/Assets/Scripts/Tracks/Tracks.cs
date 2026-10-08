using System;
using System.Collections.Generic;
using DefaultNamespace.ResourceSystem;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Tracks : MonoBehaviour, IInteractable // tracks are not collectable 
{
    [SerializeField] private GameObject[] meshs;
    private GameObject _prefab;
    [SerializeField] private GameObject menu;
    [SerializeField] private TextMeshProUGUI animalDirec;
    [SerializeField] private TextMeshProUGUI Specie;
    [SerializeField] private TextMeshProUGUI TimeStamp;
    [SerializeField] private TextMeshProUGUI state;
    private AnimalSo _animalSo;

    public bool playerInRange;


    [SerializeField] private Material glowMaterial;


    private MeshRenderer[] _meshRenderer;

    private int _maxTrackAge;
    private bool _isMenuActive;
    private Camera cam;

    [Header("Tracks data info")] private TrackData trackData = new();

    public ResourceUI resourceUI { get; }
    public bool OutlineMe { get; set; }
    public bool CanBeCollected { get; set; }
    public GameObject Gm { get; set; }
    public bool isHit { get; set; }
    public Vector3 hitPos { get; set; }


    private Material _originalMaterial;

    private void Awake()
    {
        OutlineMe = true; // we are using Ienteractable to outline 
        cam = Camera.main;
      
        _meshRenderer = GetComponentsInChildren<MeshRenderer>();
        _originalMaterial = GetComponentInChildren<MeshRenderer>().material;
        menu.SetActive(false);
        RenderGlowMaterial();
    }

    public void ExecuteAction()
    {
    }

    public bool CanDisplay { get; set; }
    public string UseMeDescription { get; set; }
    public string Description { get; set; }
    public bool CanUse { get; set; }
    public bool CanHarvest { get; set; }
    public bool CanCraft { get; set; }

    public void Craft()
    {
        throw new NotImplementedException();
    }

    public void Harvest()
    {
        throw new NotImplementedException();
    }

    public void UseMe()
    {
        throw new NotImplementedException();
    }

    private void OnEnable()
    {
        EventBus.OnToggleTracksMenu += ActivateTrackMenu;
        EventBus.OnHourChanged += OnHourChanged;
    }

    private void OnDisable()
    {
        EventBus.OnHourChanged -= OnHourChanged;
        EventBus.OnToggleTracksMenu -= ActivateTrackMenu;
    }

    private void Update()
    {
        if (menu.activeInHierarchy)
        {
            FaceCamera();
        }
    }

    private void ActivateTrackMenu()
    {
        if (!playerInRange) return;
        menu.SetActive(true);
        RenderOriginalMat();
    }

    public void Initialize(TrackData data)
    {
        trackData = data;
        _animalSo = data.soAnimal;
        _prefab = data.prefab;
        DisplayTarackData();
    }

    private void DisplayTarackData()
    {
        animalDirec.text = trackData.Dir;
        Specie.text = trackData.specie;
        TimeStamp.text = trackData.trackAge.ToString();
        state.text = trackData.AnimalState.GetType().Name;
    }

    private void OnHourChanged(int hours)
    {
        trackData.TimeStamp++;
        if (trackData.TimeStamp <= trackData.maxTrackAge)
        {
            DisplayTarackData(); // maybe update tracks if needed 
        }
        else
        {
            GlobalPool.instance.Return(_prefab, gameObject);
        }
    }

    public void Collect(PlayerInventory collector)
    {
    }

    public void ToggleMenu()
    {
    }

    void FaceCamera()
    {
        Vector3 dir = cam.transform.position - menu.transform.position;

        dir.y = 0; // Optional: keep UI upright (no vertical tilt)

        menu.transform.rotation = Quaternion.LookRotation(-dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.instance.ToggleInteractButton(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            menu.gameObject.SetActive(false);
            UIManager.instance.ToggleInteractButton(false);
        }
    }

    private void RenderOriginalMat()
    {
        foreach (var renderer in _meshRenderer)
        {
            renderer.material = _originalMaterial;
        }
    }

    private void RenderGlowMaterial()
    {
        foreach (var renderer in _meshRenderer)
        {
            renderer.material = glowMaterial;
        }
    }

    protected virtual void SetBools()
    {
        CanBeCollected = false;
        CanDisplay = false;
        Description = "Analyze";
        CanCraft = false;
        CanHarvest = false;
        OutlineMe = true;
    }
    
}