using DefaultNamespace.EventBus;
using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Obj<TSo> : MonoBehaviour, IsoInitializer<TSo>, IInteractable
{
    protected PosInChunk CashedPosInChunk;
    public GameObject Gm { get; set; }
    public bool isHit { get; set; }
    public Vector3 hitPos { get; set; }

    [Header("Ui Setting")] [SerializeField]
    private bool craft;

    [SerializeField] private bool harvest;
    [SerializeField] private bool useMe;

    public Rigidbody rb { get; set; }
    protected Camera cam;

    public ResourceUI resourceUI { get; set; }
    public bool OutlineMe { get; set; }
    public bool CanBeCollected { get; set; } // one way to do this is 
    public TSo So { get; set; }

    public bool CanCraft
    {
        get => craft;
        set => craft = value;
    }

    public bool CanHarvest
    {
        get => harvest;
        set => harvest = value;
    }

    public string UseMeDescription { get; set; }
    public string Description { get; set; }

    public bool CanUse
    {
        get => useMe;
        set => craft = value;
    }

    public InventoryItem InventoryItem { get; set; }
    public bool CanDisplay { get; set; }

    protected virtual void Awake()
    {
        Description = "Collect";
        resourceUI = GetComponent<ResourceUI>();
        cam = Camera.main;
        SetUiBools();
        CanDisplay = true;
        OutlineMe = true;
    }

    public virtual void Initialize(TSo so)
    {
        So = so;
    }

    public void SeCashedPos(PosInChunk casedPos)
    {
        CashedPosInChunk = casedPos;
    }

    public virtual void Craft()
    {
        PlayerRepository.instance.CraftWorldItem(gameObject);
    }

    public virtual void Harvest()
    {
    }


    public virtual void UseMe()
    {
        PlayerRepository.instance.RemoveResourceFromInventory(this as Obj<ObjSo>, true);
        Destroy(gameObject);
    }

    public void ExecuteAction()
    {
        HandleAction();
    }

    protected virtual void HandleAction()
    {
        EventManager.Instance.reseourceEvent.GatherResource(gameObject);
    }

    protected virtual void SetUiBools() // ToDo: show this in inspector instead of script 
    {
    }
}