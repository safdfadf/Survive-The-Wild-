using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResourceInventory : MonoBehaviour
{
    private int width = 8;
    private int height = 8;
    private int occuppiedCount = 0;

    [SerializeField] private float spacingBtwSlots;

    private InventoryItem heldItem = null;

    [SerializeField] private Slot slotPrefab;

    private Slot[,] slots;
    private Dictionary<ResourceSo, List<GameObject>> resources = new();


    private void Awake()
    {
        slots = new Slot[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var slot = Instantiate(slotPrefab, transform);
                slot.gridPosition = new Vector2Int(x, y);
                slot.transform.localPosition = new Vector3(x * spacingBtwSlots, y * spacingBtwSlots, 0);
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
        float depth = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth)
        );
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        float minX = 0;
        float maxX = (width - 1) * spacingBtwSlots;
        float minY = 0;
        float maxY = (height - 1) * spacingBtwSlots;

        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        localPos.y = Mathf.Clamp(localPos.y, minY, maxY);

        heldItem.transform.localPosition = localPos;
    
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
                    s.Valid();     // turn green
                else
                    s.Invalid();   // turn red
            }
        }
    }
    public void TryPlaceItem(ResourceSo So, GameObject itemPrefab) // called by player inventory
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
                        resources[So] = new List<GameObject>();

                    resources[So].Add(itemPrefab);
                    return;
                }
            }
        }
    }

    private bool IsAreaFree(int startX, int startY, Vector2Int size)
    {
        // Prevent overflow outside grid
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

    private void PlaceItemAt(GameObject prefab, Vector2Int position, Vector2Int size)
    {
        if (prefab == null)
        {
            Debug.Log("item is null");
        }

        InventoryItem item = prefab.GetComponent<InventoryItem>();

        occuppiedCount = 0;
        prefab.transform.SetParent(slots[position.x, position.y].transform);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        prefab.GetComponent<InventoryItem>().origin = position;
        prefab.GetComponent<InventoryItem>().size = size;


        slots[position.x, position.y].PlaceItem(prefab);

        // Mark all occupied slots
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                slots[position.x + x, position.y + y].isOccupied = true;
                slots[position.x + x, position.y + y].occupiedItem = item;
                occuppiedCount++;
            }
        }
        // add a functionality to place the item at Body Status ui 
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

    public void OnSlotClicked(Slot slot)
    {
        Debug.Log("on slot clicked");
        Vector2Int pos = slot.gridPosition;

        if (heldItem == null)
        {
            Debug.Log("heldItem is null");
            heldItem = PickUpItem(pos);
            if (heldItem == null) return;
            Collider collider = heldItem.gameObject.GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
            heldItem.transform.SetParent(transform); // follow mouse
        }
        else
        {
            if (CanPlaceItem(heldItem, pos))
            {
                Collider collider = heldItem.GetComponent<Collider>();
                collider.enabled = true;
                PlaceItemAt(heldItem.gameObject, pos, heldItem.size);
                heldItem.origin = pos;
                heldItem = null;
                ClearPreviewColors();
            }
            else
            {
                Debug.Log("Cannot place item here");
            }
        }
    }

    private Slot SlotAtCurrentPos(Vector3 localPos)
    {
        int x = Mathf.RoundToInt(localPos.x / spacingBtwSlots);
        int y = Mathf.RoundToInt(localPos.y / spacingBtwSlots);

        if (x < 0 || y < 0 || x >= width || y >= height)
            return null;

        return slots[x, y];
    }
    public void RemoveResourse(ResourceSo so)
    {
        if (!resources.ContainsKey(so)) return;
        if (resources[so].Count == 0) return;

        // Get the first item of this type
        GameObject item = resources[so][0];

        // Remove from list
        resources[so].RemoveAt(0);

        // Destroy the physical object
        GlobalPool.instance.Return(so.prefab, item);

        // If list is empty, remove key
        if (resources[so].Count == 0)
            resources.Remove(so);
    }
}