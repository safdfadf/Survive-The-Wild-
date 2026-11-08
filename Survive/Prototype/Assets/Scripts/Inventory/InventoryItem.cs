using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Vector2Int originalSize;

    // in ths inventory item how can i make it call toggle menu 
    public Vector2Int origin { get; set; }

    public RectTransform rect { get; set; }
    public Image icon { get; set; }
    public ObjSo so { get; set; }
    public GameObject gm { get; set; }

    public bool isIsolated { get; set; }
    public bool IsInCraftingList { get; set; }
    [SerializeField] protected GameObject menu;
    [SerializeField] protected Button craftButton;
    [SerializeField] protected Button harvest;
    [SerializeField] protected Button removeButton;
    public Obj<ObjSo> _currentObj { get; private set; }
    public Button useMe;

    private List<Button> _activeButtons = new();
    private float _spacingBetweenSlotX;
    private float _spacingBetweenSlotY;
    [SerializeField] private GameObject ItemGm;
    [SerializeField] private Transform parentTransform;
    private ResourceInventory _inventory;
    private bool _isMenuOpen;
    public Slot slot;
    public bool IsRotated { get; set; }

    public Vector2Int Size
    {
        get =>
            IsRotated
                ? new Vector2Int(originalSize.y, originalSize.x)
                : originalSize;
        set => value = originalSize;
    }

    private void Awake()
    {
        slot = gameObject.GetComponentInParent<Slot>();


        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        if (rect != null)
            removeButton.onClick.AddListener(Remove);
        _activeButtons = new List<Button> { craftButton, harvest, useMe, removeButton };
    }

    private void OnEnable()
    {
        SceneRoot.Instance.EventManager.UiEvents.onRemoveMenu += DeactivateMenu;
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.UiEvents.onRemoveMenu -= DeactivateMenu;
    }

    public void SetItem(Sprite sprite, GameObject Obj, ResourceInventory inventory)
    {
        _inventory = inventory;
        icon.sprite = sprite;
        gm = Obj;
        _currentObj = Obj.GetComponent<Obj<ObjSo>>();
        isIsolated = _currentObj.IsIsolated;
        rect.sizeDelta = new Vector2(
            so.size.x * 60,
            so.size.y * 60
        );
    }

    public void Craft()
    {
        IsInCraftingList = true;
        SceneRoot.Instance.EventManager.UiEvents.RemoveMenu();
        EventBus.OnCraftResource.Invoke(so, this);
        SceneRoot.Instance.EventManager.CraftEvents.PlaceInCraftMenu(this);
        SetMenuPos();
        _currentObj.CanCraft = false;
    }

    private void Remove() // only this will call to go back to the inventory 
    {
        if (isIsolated)
        {
            // Drop Game oBjetc in real world 
            Destroy(gameObject);
            return;
        }

        if (IsInCraftingList)
        {
            IsInCraftingList = false;
            _currentObj.CanCraft = true;
            SetMenuPos();
            EventBus.OnUnCraftResource.Invoke(so, this);
            SceneRoot.Instance.EventManager.CraftEvents.onRemoveFromCraftMenu(so, this);
            SceneRoot.Instance.EventManager.UiEvents.RemoveMenu();
        }
        else
        {
            SetMenuPos();
            PlayerRepository.instance.RemoveResourceFromInventory(_currentObj, true);
            // remove it from resource pool aswell
        }
    }

    public void Toggle()
    {
        _isMenuOpen = !_isMenuOpen;
        if (_isMenuOpen)
        {
            _inventory.DeactivateSubMenu();
        }
        else
        {
            ActivateMenu();
        }
    }

    private void ActivateMenu()
    {
        SetMenuPos();
        _inventory.ActivateSubMenu(Remove, Craft, _currentObj);
    }

    public void DeactivateMenu()
    {
        _isMenuOpen = false;
        _inventory.DeactivateSubMenu();
    }

    public void SetMenuPos()
    {
        var slotRect = slot.GetComponent<RectTransform>();
        var finalPos = slotRect.position + new Vector3(100, 0, 0);
        _inventory.SetMenuPos(finalPos);
    }

    // now lets talk about input 
    public void Rotate()
    {
        IsRotated = !IsRotated;
        Size = new Vector2Int(Size.y, Size.x);

        Vector2 currentSize = rect.sizeDelta;

        rect.sizeDelta = new Vector2(
            currentSize.y,
            currentSize.x);
        rect.localRotation =
            Quaternion.Euler(0, 0, IsRotated ? 90f : 0f);
    }
}