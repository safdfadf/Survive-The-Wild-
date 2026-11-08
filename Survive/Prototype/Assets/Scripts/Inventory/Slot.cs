using System;
using System.Collections.Generic;
using Player;
using SurviveTheWild.CraftingSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SlotType slotType;
    [HideInInspector] public Transform parent;
    public Vector2Int gridPosition { get; set; }
    public bool isOccupied { get; set; }
    private Transform itemAnchor;
    public List<InventoryItem> occupiedItem { get; set; }
    public float storeCapacity { get; set; }
    private Image _img;
    private Color _regularColor;
    public RectTransform rect { get; set; }

    private Sprite _currentSprite;

    //ToDo : Function for valid and invalid spots 
    public Vector3 worldPosition { get; set; }

    [SerializeField]
    public SlotType SlotType
    {
        get => slotType;
        set => slotType = value;
    }

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
        occupiedItem = new List<InventoryItem>();
        ToggleAlpha(false);
    }

    public void InitializeSlot(Vector2Int GridPosition, float X, float Y, Transform parent)
    {
        gridPosition = GridPosition;
        spacingX = X;
        spacingY = Y;
        this.parent = parent;
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
        if (eventData.button != PointerEventData.InputButton.Left) return;
        switch (SlotType)
        {
            case SlotType.Inventory:
            {
                var inventory = GetComponentInParent<ResourceInventory>();
                inventory.OnSlotClicked(this);
                break;
            }
            case SlotType.CraftingSLot:
            {
                var ui = FindAnyObjectByType<CraftingUi>();
                ui.OnClick(this);
                break;
            }
            default:
            {
                if (slotType == SlotType.EmptySpace && PlayerRepository.instance.resourceInventory.HeldItem != null)
                {
                    InventoryItem item = PlayerRepository.instance.resourceInventory.HeldItem;
                    PlayerRepository.instance.RemoveResourceFromInventory(
                        item._currentObj, false);
                    Destroy(item.gameObject);
                    PlayerRepository.instance.resourceInventory.HeldItem = null;
                }

                break;
            }
        }
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

    public void SetOriginalPos()
    {
        rect.transform.SetParent(parent);
        rect.anchoredPosition = new Vector2(
            gridPosition.x * spacingX,
            gridPosition.y * spacingY);
    }

    public bool CanOccupy()
    {
        return occupiedItem.Count < storeCapacity;
    }
}

[System.Serializable]
public enum SlotType
{
    Inventory,
    CookingSpot,
    EmptySpace,
    CraftingSLot
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