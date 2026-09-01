using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPosition { get; set; }
    public bool isOccupied { get; set; }
    private Transform itemAnchor;
    public InventoryItem occupiedItem { get; set; }

    private Image _img;
    private Color _regularColor;
    public RectTransform rect { get; set; }

    private Sprite _currentSprite;

    //ToDo : Function for valid and invalid spots 
    public Vector3 worldPosition { get; set; }
    public CookingData cookingData { get; set; }
    public int cookingSpotIndex = -1; // index in CampFire.cookingSpots

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        isOccupied = false;
        _img = GetComponentInChildren<Image>();
        _regularColor = _img.color;
        ToggleAlpha(false);
    }

    public void PlaceItem(InventoryItem item)
    {
        item.rect.SetParent(rect);
        item.rect.anchoredPosition = Vector2.zero;
        item.rect.localRotation = Quaternion.identity;

        isOccupied = true;
        occupiedItem = item;
    }

    public void RegularColor()
    {
        ToggleAlpha(false);
    }

    public void Valid()
    {
        ToggleAlpha(true);
        _img.color = Color.gray;
    }

    public void Invalid()
    {
        ToggleAlpha(true);
        _img.color = Color.red;
    }

    public void ClearSlot()
    {
        foreach (Transform child in itemAnchor)
            Destroy(child.gameObject);

        isOccupied = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ResourceInventory inventory = GetComponentInParent<ResourceInventory>();
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<ResourceInventory>();
        }

        inventory.OnSlotClicked(this);
    }

    private void ToggleAlpha(bool isOn)
    {
        Color c = _img.color;
        c.a = isOn ? 1 : 0;
        _img.color = c;
    }

    public void SetRegularColor()
    {
        ToggleAlpha(false);
    }
}

public enum SlotType
{
    Inventory,
    CookingSpot
}

public class CookingData
{
    public SlotType slotType;
    public GameObject handler;

    public CookingData(SlotType SlotType, GameObject Handler)
    {
        slotType = SlotType;
        handler = Handler;
    }
}