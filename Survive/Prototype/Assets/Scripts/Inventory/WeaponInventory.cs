using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private int maxSlots = 4;
        [SerializeField] private float spacing = 80f;
        [SerializeField] private GameObject uiWeaponPrefab;

        private RectTransform rect;
        private List<WeaponSo> storedWeapons = new();
        private List<InventoryItem> uiItems = new();

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void AddWeapon(WeaponSo so, InventoryItem item)
        {
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
                itemRect.anchoredPosition = new Vector2(i * spacing, 0);
                itemRect.localRotation = Quaternion.identity;
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
    }
}