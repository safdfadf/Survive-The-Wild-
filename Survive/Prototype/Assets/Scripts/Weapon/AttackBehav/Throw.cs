using UnityEngine;

namespace DefaultNamespace.Weapon
{
    public class Throw:ProjectileParent
    {
        // here we take weapon 
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            if(!isAiming)return;
            base.Update();
        }
        private void LateUpdate()
        {
            UpdateRestRotation();
        }
        protected override void StartAiming()
        {
            isAiming = true;
            _animator.ToggleAiming(true);
        }

        protected override void StopAiming()
        {
            isAiming = false;
            _animator.ToggleAiming(false);
        }

        protected override void UpdateRestRotation()
        {
            RestPoint = Player.rightSpwnPoint;
            base.UpdateRestRotation();
        }

        protected override void Shoot()
        {
            Vector3 dir = (aimTarget.position - RestPoint.position).normalized;
            ArrowMove arrow = weapon.gameObject.AddComponent<ArrowMove>();
            arrow.InitDamage(data.weaponSo.damage);
            arrow.ShootArrow(dir,20);
            // we need to know 
                
        }
    }
}