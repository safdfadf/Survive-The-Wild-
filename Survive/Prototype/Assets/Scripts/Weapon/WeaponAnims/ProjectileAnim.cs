using UnityEngine;

namespace SurviveTheWild.Weapon.WeaponAnims
{
    public class ProjectileAnim : WeaponAnimations
    {
        public void ToggleAiming(bool isAiming)
        {
            animator.Aim(isAiming);
        }

        public void Shoot()
        {
            animator.FireArrow(true);
   
        }

        public void RestShootable()
        {
            StartCoroutine(animator.ResetFireArrow());
        }

        public void UnEquip()
        {
            animator.HandWeaponEquip(false);
        }
        
    }
}