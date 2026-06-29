using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour,IPointerClickHandler
{
    public Vector2Int gridPosition { get;set; }
    public bool isOccupied { get;  set; }
    public Transform itemAnchor;
    public InventoryItem occupiedItem;

  
    private void Awake()
    {
        isOccupied = false;
    }

    public void PlaceItem(GameObject item)
    {
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        isOccupied = true;
    }

    public void ClearSlot()
    {
        foreach (Transform child in itemAnchor)
            Destroy(child.gameObject);

        isOccupied = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)return;
        ResourceInventory inventory = GetComponentInParent<ResourceInventory>();
        if(inventory == null){Debug.Log("Cant fint inventory");return;}
        inventory.OnSlotClicked(this);
    }

}


