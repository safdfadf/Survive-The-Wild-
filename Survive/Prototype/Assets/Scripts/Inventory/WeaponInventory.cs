using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private int maxSlots = 4;
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
                Debug.Log("Weapon inventory full");
                return;
            }

            storedWeapons.Add(so);
            uiItems.Add(item);

            // Assign item to slot
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
        }

        private void AlignUIItems()
        {
            for (int i = 0; i < uiItems.Count; i++)
            {
                RectTransform itemRect = uiItems[i].rect;
                RectTransform childObj = itemRect.GetChild(0).GetComponent<RectTransform>();
                Image img = uiItems[i].GetComponent<Image>();
                img.enabled = false;

                AvailablePos slot = GetAvailableSlots();
                if (slot == null)
                {
                    Debug.LogWarning("No available slot for item: " + uiItems[i].name);
                    return;
                }

                slot.item = uiItems[i];

                RectTransform t = slot.pos.GetComponent<RectTransform>();
                Image childI = childObj.GetComponent<Image>();
                childI.sprite = img.sprite;
                childI.enabled = true;

                itemRect.SetParent(t);
                itemRect.anchoredPosition = Vector2.zero;
                childObj.anchoredPosition = Vector2.zero;
                childObj.localRotation = Quaternion.identity;
                WeaponSo so = uiItems[i].so as WeaponSo;
                childObj.localScale = Vector3.one * so.scale;
            }
        }

        public void RemoveWeapon(WeaponSo so) // remove inventory prefab of the weapon 
        {
            int index = storedWeapons.IndexOf(so);
            if (index < 0) return;

            currentItem = uiItems[index];
            storedWeapons.RemoveAt(index);
            uiItems.RemoveAt(index);
            currentItem.gameObject.SetActive(false);
            FreeSlot(currentItem);
            // Destroy(uiItems[index].gameObject);

            AlignUIItems();
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
                UnequipCurrentWeapon();
                Debug.Log("Slot empty → player empty-handed");
                return;
            }

            InventoryItem nextItem = nextSlot.item;
            BaseWeapon weapon = nextItem._currentObj.GetComponent<BaseWeapon>();

            UnequipCurrentWeapon();

            weapon.isEquipped = true;
            weapon.UseMe();
        }

        private void UnequipCurrentWeapon()
        {
            foreach (var s in slots)
            {
                if (s.item != null)
                {
                    BaseWeapon w = s.item._currentObj.GetComponent<BaseWeapon>();
                    if (w.isEquipped)
                    {
                        w.isEquipped = false;
                        FreeSlot(s.item);
                    }
                }
            }
        }

        private int GetCurrentIndex()
        {
            return uiItems.FindIndex(item =>
            {
                BaseWeapon weapon = item._currentObj.GetComponent<BaseWeapon>();
                return weapon.isEquipped;
            });
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