using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Obj<TSo> : MonoBehaviour, IsoInitializer<TSo>, ICollectable, IAction
{
    protected PosInChunk CashedPosInChunk;
    public GameObject Gm { get; set; }

    public Rigidbody rb { get; set; }
    protected Camera cam;

    public ResourceUI resourceUI { get; set; }
    public bool outlineMe { get; set; }
    public bool canBeCollected { get; set; }
    public TSo So { get; set; }

    public bool canCraft { get; set; }
    public bool canHarvest { get; set; }
    public string useMeDescription { get; set; }
    public string Description { get; set; }
    public bool canUse { get; set; }
    public GameObject obj { get; set; }

    public InventoryItem InventoryItem { get; set; }

    protected virtual void Awake()
    {
        Description = "Collect";
        outlineMe = true;
        canBeCollected = true;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        resourceUI = GetComponent<ResourceUI>();
        cam = Camera.main;
        SetUiBools();
        obj = this.gameObject;
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
    }


    protected virtual void SetUiBools()
    {
        canCraft = true;
        canHarvest = true;
        canUse = true;
    }
}