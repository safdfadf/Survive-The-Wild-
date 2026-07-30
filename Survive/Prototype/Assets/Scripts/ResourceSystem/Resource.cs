using Player;
using UnityEngine;
using UnityEngine.UI;

public abstract class Resource<TSo> : MonoBehaviour, IsoInitializer<TSo>, ICollectable
{
    [SerializeField] protected GameObject menu;
    [SerializeField] protected Button craftButton;
    [SerializeField] protected Button harvest;
    [SerializeField] protected Button removeButton;
    protected Mesh OriginalMesh;
    protected PosInChunk CashedPosInChunk;
    public GameObject Gm { get; set; }
    protected bool _isInCraftingList;
    protected Rigidbody rb;
    protected Camera cam;

    public bool canBeCollected { get; set; }
    public TSo So { get; set; }

    protected virtual void Awake()
    {
        canBeCollected = true;

        if (menu != null)
            menu.SetActive(false);

        craftButton?.onClick.AddListener(CraftMe);
        UpdateRemoveButton();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        OriginalMesh = meshFilter.mesh;
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

    public virtual void Collect(PlayerInventory collector)
    {
        canBeCollected = false;
        collector.AddResource(this);
        if(CashedPosInChunk == null)return;
        CashedPosInChunk.IsAvailable = true;
        CashedPosInChunk.LastSpawnedSo = null;
        CashedPosInChunk.IsPersistent = false;
    }

    protected virtual void CraftMe()
    {
        Debug.Log("Crafting");
        if (So == null) return;
        Debug.Log("So null");
        _isInCraftingList = true;
        UpdateRemoveButton();
    }

    protected void UpdateRemoveButton()
    {
        removeButton?.onClick.RemoveAllListeners();

        if (_isInCraftingList)
            removeButton?.onClick.AddListener(RemoveMeCraftingList);
        else
            removeButton?.onClick.AddListener(RemoveFromInventory);
    }

    protected virtual void RemoveMeCraftingList()
    {
    }

    protected virtual void RemoveFromInventory()
    {
        if (_isInCraftingList) return;
    }

    public void ToggleMenu()
    {
        menu?.SetActive(!menu.activeSelf);
    }

    public void SetKinematic(bool isKinematic)
    {
        rb.isKinematic = isKinematic;
    }
}