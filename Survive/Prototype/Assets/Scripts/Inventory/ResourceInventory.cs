using System.Collections.Generic;
using FoodSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class ResourceInventory : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    private RectTransform inventoryRect;

    [FormerlySerializedAs("spacingBtwSlots")] [SerializeField]
    private float spacingBtwSlotsX;

    [SerializeField] private float spacingBtwSlotsY;

    private InventoryItem heldItem = null;

    [SerializeField] private Slot slotPrefab;

    [SerializeField] private GameObject parent;
    private Slot[,] slots;
    private Dictionary<ObjSo, List<InventoryItem>> resources = new();

    private void Awake()
    {
        slots = new Slot[width, height];
        inventoryRect = parent.GetComponent<RectTransform>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Slot slot = Instantiate(slotPrefab, parent.transform);
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

    private void UpdateHeldItemPos() // this is updated based on held item's pos 
    {
        ClearPreviewColors();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)parent.transform,
            Input.mousePosition,
            null,
            out localPos
        );

        heldItem.rect.anchoredPosition = localPos;
        Vector2Int origin = GridPosFromLocalPos(localPos);
        if (origin.x < 0 || origin.y < 0 || origin.x >= width || origin.y >= height)
            return;

        PreviewPlacement(heldItem, origin);
    }

    private Vector2Int GridPosFromLocalPos(Vector2 localPos)
    {
        var x = Mathf.FloorToInt(localPos.x / spacingBtwSlotsX);
        var y = Mathf.FloorToInt(localPos.y / spacingBtwSlotsY);

        return new Vector2Int(x, y);
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
                if (origin.x + x > width || origin.y + y > height) return;
                Slot s = slots[origin.x + x, origin.y + y];

                if (canPlace)
                    s.Valid(); 
                else
                    s.Invalid();
            }
        }
    }

    public void TryPlaceItem(ObjSo So, InventoryItem itemPrefab) // called by player inventory
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
                    return;
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
                    return false; // continue 
            }
        }

        return true;
    }

    private void PlaceItemAt(InventoryItem item, Vector2Int position, Vector2Int size)
    {
        Debug.Log("try place item   ");
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
            foreach (var img in heldItem.GetComponentsInChildren<Image>())
                img.raycastTarget = false;
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
            Obj<ObjSo> food = obj.GetComponent<Obj<ObjSo>>();
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
        int y = Mathf.RoundToInt(localPos.y / spacingBtwSlotsY);

        if (x < 0 || y < 0 || x >= width || y >= height)
            return null;

        return slots[x, y];
    }

    public InventoryItem GetInventoryItem(InventoryItem i)
    {
        foreach (var items in resources.Values)
        {
            foreach (var item in items)
            {
                if (item == i)
                    return item;
            }
        }

        return null;
    }

    public void RemoveResourse(ObjSo so)
    {
        if (!resources.ContainsKey(so)) return;
        if (resources[so].Count == 0) return;

        InventoryItem item = resources[so][0];
        resources[so].RemoveAt(0);

        if (resources[so].Count == 0)
            resources.Remove(so);
    }
}