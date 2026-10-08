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
    public float spacingX { get; set; }
    public float spacingY { get; set; }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        isOccupied = false;
        _img = GetComponentInChildren<Image>();
        _regularColor = _img.color;
        ToggleAlpha(false);
    }

    public void InitializeSlot(Vector2Int GridPosition, float X, float Y)
    {
        gridPosition = GridPosition;
        spacingX = X;
        spacingY = Y;
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

    public void ToggleRaycastTarget()
    {
        _img.raycastTarget = !_img.raycastTarget;
        print(_img.raycastTarget);
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

    public void ToggleAlpha(bool isOn)
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