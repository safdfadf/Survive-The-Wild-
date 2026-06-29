using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class WeaponInventory:MonoBehaviour
    {
        [SerializeField] private readonly int _maxSlots = 4;
        [SerializeField] private float spacing = .5f;
        private List<BaseWeapon>_availableWeapons = new();
        
        public void AddWeapon(BaseWeapon weapon)
        {
            if (_availableWeapons.Count > _maxSlots)
            {
                Debug.Log("inventory limit reached");
               
                return;
            }
            _availableWeapons.Add(weapon);
            weapon.transform.SetParent(transform,true);
            AlignWeapons(weapon);
        }
        private void AlignWeapons(BaseWeapon weapon)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.localPosition = new Vector3(i * spacing, 0, 0);
                child.localRotation = Quaternion.Euler(weapon.inventoryRotAngle, 0, 0);
            }
        }
        public void RemoveWeapon(BaseWeapon weapon)
        {
            _availableWeapons.Remove(weapon);
             weapon.DestroyMe();
        }
    }
}