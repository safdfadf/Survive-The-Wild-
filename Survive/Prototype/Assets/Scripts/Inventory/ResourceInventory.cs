using System.Collections.Generic;
using FoodSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ResourceInventory : MonoBehaviour
{
    //ToDo convert 3d to 2d inventory 
    private int width = 8;
    private int height = 8;
    private RectTransform inventoryRect;

    [FormerlySerializedAs("spacingBtwSlots")] [SerializeField]
    private float spacingBtwSlotsX;

    [SerializeField] private float spacingBtwSlotsY;

    private InventoryItem heldItem = null;

    [SerializeField] private Slot slotPrefab;


    private Slot[,] slots;
    private Dictionary<ResourceSo, List<InventoryItem>> resources = new();

    private void Awake()
    {
        slots = new Slot[width, height];
        inventoryRect = GetComponent<RectTransform>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Slot slot = Instantiate(slotPrefab, transform);
                if (slot == null)

                {
                    Debug.LogWarning("Slot is null");
                    return;
                }

                slot.gridPosition = new Vector2Int(x, y);
                slot.rect = slot.gameObject.GetComponent<RectTransform>();
                slot.rect.anchoredPosition = new Vector2(
                    x * spacingBtwSlotsX,
                    y * spacingBtwSlotsY
                );
                slot.cookingData = null;
                slots[x, y] = slot;
            }
        }
    }

    private void LateUpdate()
    {
        if (heldItem == null) return;
        UpdateHeldItemPos();
    }

    private void UpdateHeldItemPos()
    {
        ClearPreviewColors();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform,
            Input.mousePosition,
            null,
            out localPos
        );

        heldItem.rect.anchoredPosition = localPos;

        Slot slot = SlotAtCurrentPos(localPos);
        if (slot == null) return;

        PreviewPlacement(heldItem, slot.gridPosition);
    }

    private void ClearPreviewColors()
    {
        foreach (var slot in slots)
            slot.SetRegularColor();
    }

    private void PreviewPlacement(InventoryItem item, Vector2Int origin)
    {
        Vector2Int size = item.size;

        bool canPlace = IsAreaFree(origin.x, origin.y, size);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Slot s = slots[origin.x + x, origin.y + y];

                if (canPlace)
                    s.Valid(); // turn green
                else
                    s.Invalid(); // turn red
            }
        }
    }

    public void TryPlaceItem(ResourceSo So, InventoryItem itemPrefab) // called by player inventory
    {
        Vector2Int size = So.size;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var slot = slots[x, y];
                if (IsAreaFree(x, y, size))
                {
                    PlaceItemAt(itemPrefab, new Vector2Int(x, y), size);
                    if (!resources.ContainsKey(So))
                        resources[So] = new List<InventoryItem>();

                    resources[So].Add(itemPrefab);
                }
            }
        }
    }

    private bool IsAreaFree(int startX, int startY, Vector2Int size)
    {
        if (startX + size.x > width) return false;
        if (startY + size.y > height) return false;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (slots[startX + x, startY + y].isOccupied)
                    return false;
            }
        }

        return true;
    }

    private bool IsOutOFBounds(int startX, int startY, Vector2Int size)
    {
        if (startX + size.x > width) return true;
        if (startY + size.y > height) return true;
        return false;
    }

    private void PlaceItemAt(InventoryItem item, Vector2Int position, Vector2Int size)
    {
        item.rect.SetParent(slots[position.x, position.y].rect);
        item.rect.anchoredPosition = Vector2.zero;
        item.rect.localRotation = Quaternion.identity;

        item.origin = position;

        for (int x = 0; x < size.x; x++)
        for (int y = 0; y < size.y; y++)
        {
            slots[position.x + x, position.y + y].isOccupied = true;
            slots[position.x + x, position.y + y].occupiedItem = item;
        }
    }

    private void ClearArea(Vector2Int origin, Vector2Int size, InventoryItem item)
    {
        for (int x = 0; x < size.x; x++)
        for (int y = 0; y < size.y; y++)
        {
            var s = slots[origin.x + x, origin.y + y];
            if (s.occupiedItem == item) // only clear if it's the same item
            {
                s.isOccupied = false;
                s.occupiedItem = null;
            }
        }
    }

    private InventoryItem PickUpItem(Vector2Int gridPos)
    {
        Debug.Log("pick up item");
        Slot slot = slots[gridPos.x, gridPos.y];
        if (!slot.isOccupied || slot.occupiedItem == null)
            return null;

        InventoryItem item = slot.occupiedItem;

        ClearArea(item.origin, item.size, item);

        return item;
    }

    private bool CanPlaceItem(InventoryItem item, Vector2Int gridPos)
    {
        if (!IsAreaFree(gridPos.x, gridPos.y, item.size))
            return false;

        return true;
    }

    public void OnSlotClicked(Slot slot) // here we can check if the clicked slot is a cokking slot 
    {
        Vector2Int pos = slot.gridPosition;

        if (heldItem == null)
        {
            heldItem = PickUpItem(pos);
            if (heldItem == null) return;

            heldItem.rect.SetParent(inventoryRect);
            heldItem.rect.SetAsLastSibling(); // keep on top
            return;
        }

        if (CanPlaceItem(heldItem, pos))
        {
            PlaceItemAt(heldItem, pos, heldItem.size);
            heldItem.origin = pos;
            heldItem = null;
            ClearPreviewColors();
        }
        else if (slot.cookingData != null)
        {
            Debug.Log("slot is cooking slot");
            GameObject obj = Instantiate(heldItem.so.prefab, slot.worldPosition, Quaternion.identity);
            Obj<ResourceSo> food = obj.GetComponent<Obj<ResourceSo>>();
            food.So = heldItem.so;
            ICook cook = slot.cookingData.handler.GetComponent<ICook>();
            cook.ExecuteCooking(food as Food);
        }
        else
        {
            Debug.Log("Cannot place item here");
        }
    }

    private Slot SlotAtCurrentPos(Vector3 localPos)
    {
        int x = Mathf.RoundToInt(localPos.x / spacingBtwSlotsX);
        int y = Mathf.RoundToInt(localPos.y / spacingBtwSlotsX);

        if (x < 0 || y < 0 || x >= width || y >= height)
            return null;

        return slots[x, y];
    }

    public void RemoveResourse(ResourceSo so)
    {
        if (!resources.ContainsKey(so)) return;
        if (resources[so].Count == 0) return;

        InventoryItem item = resources[so][0];
        resources[so].RemoveAt(0);

        // drop the object in the real world

        if (resources[so].Count == 0)
            resources.Remove(so);
    }
}