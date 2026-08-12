using Player;
using UnityEngine;
using UnityEngine.UI;

public abstract class Obj<TSo> : MonoBehaviour, IsoInitializer<TSo>, ICollectable
{
    protected PosInChunk CashedPosInChunk;
    public GameObject Gm { get; set; }
  
    public Rigidbody rb { get; set; }
    protected Camera cam;
    public bool canUseButton { get; set; }
    public bool canBeCollected { get; set; }
    public TSo So { get; set; }

    protected virtual void Awake()
    {
        canBeCollected = true;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        cam = Camera.main;
    }

    public virtual void Initialize(TSo so)
    {
        So = so;
    }

    public void SeCashedPos(PosInChunk casedPos)
    {
        CashedPosInChunk = casedPos;
    }
    
    public virtual void UseMe()
    {
        
    }
    public void ToggleMenu()
    {
     //   _inventoryItem.Toggle();
    }
}