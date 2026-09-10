using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<AvailablePos> slots;
        private AvailablePos _lastSlot;
        private RectTransform rect;
        private List<WeaponSo> storedWeapons = new();
        private List<InventoryItem> uiItems = new();
        private InventoryItem currentItem;
        private int currentSlotIndex = -1;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void AddWeapon(WeaponSo so, InventoryItem item)
        {
            AvailablePos freeSlot = slots.FirstOrDefault(s => s.available);

            if (freeSlot == null)
            {
                Debug.Log("slot was null ");
                return;
            }


            freeSlot.available = false;
            freeSlot.item = item;

            // Move UI object into slot
            RectTransform itemRect = item.rect;
            RectTransform childObj = itemRect.GetChild(0).GetComponent<RectTransform>();
            Image img = item.GetComponent<Image>();
            img.enabled = false;

            RectTransform slotRect = freeSlot.pos.GetComponent<RectTransform>();
            Image childImg = childObj.GetComponent<Image>();
            childImg.sprite = img.sprite;
            childImg.enabled = true;

            itemRect.SetParent(slotRect);
            itemRect.anchoredPosition = Vector2.zero;
            childObj.anchoredPosition = Vector2.zero;
            childObj.localRotation = Quaternion.identity;

            WeaponSo wso = item.so as WeaponSo;
            childObj.localScale = Vector3.one * wso.scale;
            Debug.Log(item._currentObj + "try adding");
        }


        public void EquipNextWeapon(bool scrollUp)
        {
            int slotCount = slots.Count;

            if (slotCount == 0) return;

            // If no weapon equipped yet → start from slot 0

            if (currentSlotIndex == -1)
                currentSlotIndex = 0;
            else
                currentSlotIndex = scrollUp
                    ? (currentSlotIndex + 1) % slotCount
                    : (currentSlotIndex - 1 + slotCount) % slotCount;

            AvailablePos nextSlot = slots[currentSlotIndex];

            if (nextSlot.item == null)
            {
                Debug.Log("Slot empty → player empty-handed");
                return;
            }
            InventoryItem nextItem = nextSlot.item;
            BaseWeapon weapon = nextItem._currentObj.GetComponent<BaseWeapon>();
            weapon.UseMe();
            FreeSlot(nextItem); // remove equipped item's inventory ui 
        }

       
        private AvailablePos GetAvailableSlots()
        {
            foreach (var s in slots)
            {
                if (!s.available) continue;
                s.available = false;
                return s;
            }

            return null;
        }

        private void FreeSlot(InventoryItem item)
        {
            foreach (var s in slots)
            {
                if (s.item == item)
                {
                    s.available = true;
                    s.item = null;
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class AvailablePos
{
    public RectTransform pos;
    public bool available = true;
    public InventoryItem item;
}