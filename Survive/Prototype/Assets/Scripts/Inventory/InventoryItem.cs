using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    // in ths inventory item how can i make it call toggle menu 
    public Vector2Int origin { get; set; }
    public Vector2Int size { get; set; }
    public RectTransform rect { get; set; }
    public Image icon { get; set; }
    public ObjSo so { get; set; }
    public GameObject gm { get; set; }

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

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        if (rect != null)
            removeButton.onClick.AddListener(Remove);
        _activeButtons = new List<Button> { craftButton, harvest, useMe, removeButton };
    }

    public void SetItem(Sprite sprite, GameObject Obj, ResourceInventory inventory)
    {
        _inventory = inventory;
        icon.sprite = sprite;
        gm = Obj;
        _currentObj = Obj.GetComponent<Obj<ObjSo>>();
        rect.sizeDelta = new Vector2(
            so.size.x * 60,
            so.size.y * 60
        );
    }

    public void Craft()
    {
        IsInCraftingList = true;
        EventBus.OnCraftResource.Invoke(so, this);
    }

    private void Remove() // only this will call to go back to the inventory 
    {
        if (IsInCraftingList)
        {
            IsInCraftingList = false;
            EventBus.OnUnCraftResource.Invoke(so, this);
        }
        else
        {
            PlayerRepository.instance.RemoveResourceFromInventory(_currentObj, true);
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
        Slot slot = gameObject.GetComponentInParent<Slot>();
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        Vector3 finalPos = slotRect.position + new Vector3(100, 0, 0);
        _inventory.SetMenuPos(finalPos);
    }
}