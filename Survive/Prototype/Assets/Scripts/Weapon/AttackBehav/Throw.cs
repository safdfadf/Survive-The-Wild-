using UnityEngine;

namespace DefaultNamespace.Weapon
{
    public class Throw : ProjectileParent
    {
        [SerializeField] private Transform rayOrigin;
        private Vector3 direction = new Vector3(-65f, 0f, 180f);
        private Quaternion originalDirection;

        protected override void Awake()
        {
            RestPoint = gameObject.transform;
            base.Awake();
        }

        // how can i make sure weapon pointing correctly  
        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        protected override void Update()
        {
            if (!isAiming) return;
            base.Update();
            UpdateRestRotation();
        }

        protected override void StartAiming()
        {
            isAiming = true;
            weapon.crosshair.SetActive(true);
            _animator.ToggleAiming(true);
        }

        protected override void StopAiming()
        {
            Debug.Log("stop aiming");
            isAiming = false;
            weapon.crosshair.SetActive(false);
            _animator.ToggleAiming(false);
        }

        protected override void UpdateRestRotation()
        {
            Vector3 dir = (aimTarget.position - weapon.transform.position).normalized;

            Quaternion correction = Quaternion.Euler(90f, 0f, 0f);

            Quaternion lookRot = Quaternion.LookRotation(dir) * correction;

            weapon.transform.rotation = Quaternion.Slerp(
                weapon.transform.rotation,
                lookRot,
                Time.deltaTime * 10f);
        }

        protected override void Shoot()
        {
            Vector3 dir = (aimTarget.position - weapon.transform.position).normalized;
            RestPoint = gameObject.transform;
            ArrowMove spear = weapon.gameObject.AddComponent<ArrowMove>();
            spear.rayOrigin = rayOrigin;
            spear.InitDamage(data.weaponSo.damage);
            weapon.transform.SetParent(null);
            spear.transform.rotation = weapon.transform.rotation;
            spear.ShootArrow(dir, 20);

            _animator.Shoot();
        }

        public override void OnEquip()
        {
            weapon.transform.rotation = Quaternion.Euler(direction);
            base.OnEquip();
        }

        public override void OnUnEquip()
        {
            weapon.transform.rotation = originalDirection;
            originalDirection = weapon.transform.rotation;
            base.OnUnEquip();
        }
    }
}