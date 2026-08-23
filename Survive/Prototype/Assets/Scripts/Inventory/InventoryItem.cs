using System;
using Player;
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
    public GameObject obj { get; set; }
    
    public bool IsInCraftingList { get; set; }
    [SerializeField] protected GameObject menu;
    [SerializeField] protected Button craftButton;
    [SerializeField] protected Button harvest;
    [SerializeField] protected Button removeButton;
    public Button useMe;
    bool canUseMe = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        craftButton.onClick.AddListener(Craft);
        harvest.onClick.AddListener(Harvest);
        removeButton.onClick.AddListener(Remove);
        Toggle();
    }

    public void SetItem(Sprite sprite, GameObject Obj)
    {
        icon.sprite = sprite;
        obj = Obj;
    }

    public void Craft()
    {
        IsInCraftingList = true;
        EventBus.OnResourceAdd.Invoke(so, this);
    }

    public void Harvest()
    {
    }

    private void Remove()
    {
        if (IsInCraftingList)
        {
            IsInCraftingList = false;
            EventBus.OnResourceRemove.Invoke(so, this);
        }
        else
        {
            PlayerRepository.instance.RemoveResourceFromInventory(so, this.gameObject);
            Destroy(gameObject);// destroy inventory item 
        }
    }

    public void Toggle()
    {
        menu?.SetActive(!menu.activeSelf);
        SetUseMe(canUseMe);
    }

    public void SetUseMe(bool value) // alternative when enabled make it child of option 
    {
        canUseMe = value;
        useMe.gameObject.SetActive(value);
    }

    public void UseMeFunctionality(UnityAction call)
    {
        useMe.onClick.AddListener(() => UseMe(call));
    }

    private void UseMe(UnityAction call)
    {
        if (obj != null && !obj.activeSelf)
        {
            obj.SetActive(true);
        }

        Toggle();
        call?.Invoke();
    }
}