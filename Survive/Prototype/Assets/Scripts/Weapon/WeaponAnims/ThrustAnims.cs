using UnityEngine;

namespace DefaultNamespace.Weapon.WeaponAnims
{
    public class ThrustAnims : WeaponAnimations
    {
        public void EquipSpear()
        {
            animator.HandWeaponEquip(true);
        }

        public void UnEquipSpear()
        {
            animator.HandWeaponEquip(true);
        }

        public void ThrustAnim()
        {
            animator.Stab();
        }
    }
}