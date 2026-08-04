using System;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Vector2Int origin { get; set; }
    public Vector2Int size { get; set; }
    public RectTransform rect { get; set; }
    public Image icon { get; set; }
    public ResourceSo so { get; set; }
    public bool IsInCraftingList { get; set; }
    [SerializeField] protected GameObject menu;
    [SerializeField] protected Button craftButton;
    [SerializeField] protected Button harvest;
    [SerializeField] protected Button removeButton;
    public Button useMe; 
    bool canUseMe = false;
    
    // this script will also hold the function for craft and remove 
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        craftButton.onClick.AddListener(Craft);
        harvest.onClick.AddListener(Harvest);
        removeButton.onClick.AddListener(Remove);
        Toggle();
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void Craft()
    {
        IsInCraftingList = true;
       EventBus.OnResourceAdd.Invoke(so,this);
    }

    public void Harvest()
    {
        
    }

    public void Remove()
    {
      if (IsInCraftingList)
      {
          IsInCraftingList = false;
          EventBus.OnResourceRemove.Invoke(so, this);
      }
      else
      {
          PlayerRepository.instance.RemoveResourceFromInventory(so,this.gameObject);
      }
      
    }
    public void Toggle()
    {
        menu?.SetActive(!menu.activeSelf);
        SetUseMe(canUseMe);
    }

    public void SetUseMe(bool value)// alternative when enabled make it child of option 
    {
        canUseMe = value;
        useMe.gameObject.SetActive(value);
    }

    public void UseMeFunctionality(UnityAction call)
    {
        useMe.onClick.AddListener(call);
    }  
}