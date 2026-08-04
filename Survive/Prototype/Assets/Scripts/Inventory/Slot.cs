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

    private void Awake()
    {
        isOccupied = false;
        _img = GetComponentInChildren<Image>();
        _regularColor = _img.color;
    }

    public void PlaceItem(InventoryItem item)
    {
        item.rect.SetParent(rect);
        item.rect.anchoredPosition = Vector2.zero;
        item.rect.localRotation = Quaternion.identity;

        isOccupied = true;
        occupiedItem = item;
    }

    public void Valid()
    {
        _img.color = Color.green;
    }

    public void Invalid()
    {
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
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ResourceInventory inventory = GetComponentInParent<ResourceInventory>();
        if (inventory == null)
        {
            Debug.Log("Cant find inventory");
            return;
        }

        inventory.OnSlotClicked(this);
    }

    public void SetRegularColor()
    {
        _img.color = _regularColor;
    }
}