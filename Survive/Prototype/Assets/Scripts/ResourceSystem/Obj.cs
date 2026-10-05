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


    public Rigidbody rb { get; set; }
    protected Camera cam;

    public ResourceUI resourceUI { get; set; }
    public bool outlineMe { get; set; }
    public bool canBeCollected { get; set; } // one way to do this is 
    public TSo So { get; set; }

    public bool canCraft { get; set; }
    public bool canHarvest { get; set; }
    public string useMeDescription { get; set; }
    public string Description { get; set; }
    public bool canUse { get; set; }
    public InventoryItem InventoryItem { get; set; }
    public bool canDisplay { get; set; }

    protected virtual void Awake()
    {
        Description = "Collect";
        resourceUI = GetComponent<ResourceUI>();
        cam = Camera.main;
        SetUiBools();
        canDisplay = true;
        outlineMe = true;
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
        canCraft = true;
        canHarvest = true;
        canUse = true;
    }
}