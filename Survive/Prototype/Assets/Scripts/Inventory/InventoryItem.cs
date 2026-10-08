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

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        if (rect != null)
            removeButton.onClick.AddListener(Remove);
        _activeButtons = new List<Button> { craftButton, harvest, useMe, removeButton };
    }

    public void SetItem(Sprite sprite, GameObject Obj)
    {
        icon.sprite = sprite;
        gm = Obj;
        _currentObj = Obj.GetComponent<Obj<ObjSo>>();
        if (_currentObj.CanCraft)
        {
            craftButton.onClick.AddListener(Craft);
            _activeButtons.Add(craftButton);
        }

        if (_currentObj.CanHarvest)
        {
            harvest.onClick.AddListener(_currentObj.Harvest);
            _activeButtons.Add(harvest);
        }

        if (_currentObj.CanUse)
        {
            _activeButtons.Add(useMe);
            TextMeshProUGUI textMesh = useMe.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _currentObj.UseMeDescription;
            useMe.onClick.AddListener(() => _currentObj.UseMe());
        }

        // Increase the size 
        rect.sizeDelta = new Vector2(
            so.size.x * 60,
            so.size.y * 60
        );
        // why do we multiply it with space between spaces 
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
        menu?.SetActive(!menu.activeSelf);
        EventManager.Instance.reseourceEvent.MenuToggle();
    }
}