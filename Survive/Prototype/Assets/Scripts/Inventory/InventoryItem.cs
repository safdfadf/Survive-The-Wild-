using System;
using System.Collections.Generic;
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
    private Obj<ObjSo> _currentObj;
    public Button useMe;

    private List<Button> _activeButtons = new();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        removeButton.onClick.AddListener(Remove);
        _activeButtons = new List<Button> { craftButton, harvest, useMe, removeButton };
        Toggle();
    }

    public void SetItem(Sprite sprite, GameObject Obj)
    {
        icon.sprite = sprite;
        gm = Obj;
        _currentObj = Obj.GetComponent<Obj<ObjSo>>();
        if (_currentObj.canCraft)
        {
            craftButton.onClick.AddListener(Craft);
            _activeButtons.Add(craftButton);
        }

        if (_currentObj.canHarvest)
        {
            craftButton.onClick.AddListener(_currentObj.Harvest);
            _activeButtons.Add(harvest);
        }

        if (_currentObj.canUse)
        {
            _activeButtons.Add(useMe);
            TextMeshProUGUI textMesh = useMe.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = _currentObj.useMeDescription;
            useMe.onClick.AddListener(() => _currentObj.UseMe());
        }
    }

    public void Craft()
    {
        IsInCraftingList = true; // we might need to move this 
        EventBus.OnCraftResource.Invoke(so, this);
    }

    public void Harvest()
    {
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
    }
}