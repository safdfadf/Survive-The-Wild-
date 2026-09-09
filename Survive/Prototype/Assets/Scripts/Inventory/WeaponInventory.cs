using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private int maxSlots = 4;
        [SerializeField] private List<AvailablePos> slots;
        private RectTransform rect;
        private List<WeaponSo> storedWeapons = new();
        private List<InventoryItem> uiItems = new();

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void AddWeapon(WeaponSo so, InventoryItem item)
        {
            Debug.Log("add weapon");
            if (storedWeapons.Count >= maxSlots)
            {
                Debug.Log("Weapon inventory full");
                return;
            }

            item.rect.SetParent(transform);
            storedWeapons.Add(so);
            uiItems.Add(item);
            AlignUIItems();
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

        public void RemoveWeapon(WeaponSo so)
        {
            int index = storedWeapons.IndexOf(so);
            if (index < 0) return;

            storedWeapons.RemoveAt(index);

            Destroy(uiItems[index].gameObject);
            uiItems.RemoveAt(index);

            AlignUIItems();
        }

        public BaseWeapon SpawnEquippedWeapon(WeaponSo so)
        {
            GameObject prefab = so.prefab;
            GameObject instance = Instantiate(prefab);

            BaseWeapon weapon = instance.GetComponent<BaseWeapon>();
            //  weapon.SetWeaponSo(so);

            return weapon;
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
    }
}

[System.Serializable]
public class AvailablePos
{
    public RectTransform pos;
   [HideInInspector] public bool available = true;
}