using UnityEngine;

namespace DefaultNamespace.Weapon.WeaponAnims
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
    }
}