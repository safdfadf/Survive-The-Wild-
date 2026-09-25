using System;
using DefaultNamespace.Weapon.WeaponAnims;
using Player;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DefaultNamespace.Weapon
{
    public class ProjectileAttack : ProjectileParent
    {
        protected float drawTime;
        [SerializeField] protected float maxDrawTime = 1f;

        [SerializeField] private float _mixArrowSpeed = 0f;
        [SerializeField] private float _maxArrowSpeed = 50;
        private PlayerInventory playerInventory;
        [SerializeField] private ObjSo shootables;


        public GameObject CurrentArrow { get; private set; }


        protected override void Update()
        {
            if (!isAiming || CurrentArrow == null || !weapon.isEquipped) return;
            PullArrow();
            base.Update();
            UpdateRestRotation();
        }


        private void PullArrow()
        {
            drawTime += Time.deltaTime;
            drawTime = Mathf.Clamp(drawTime, 0f, maxDrawTime);
            float drawPercent = Mathf.Clamp01(drawTime / maxDrawTime);
            float drawOffset = Mathf.Lerp(0f, -0.5f, drawPercent);
            CurrentArrow.transform.localPosition = new Vector3(0f, 0f, drawOffset);
        }

        private void PrepareNextArrow()
        {
            CurrentArrow = PlayerRepository.instance.GetResource(shootables);
            if (CurrentArrow == null) return;
            ArrowMove Arrow = CurrentArrow.GetComponent<ArrowMove>();
            if (Arrow == null)
            {
                Debug.Log("no arrow found");
            }

            Arrow.canBeCollected = false;
            CurrentArrow.transform.SetParent(RestPoint, false);
            CurrentArrow.transform.localPosition = Vector3.zero;
            CurrentArrow.transform.localRotation = Quaternion.identity;
            CurrentArrow.SetActive(true);
        }

        protected override void StartAiming()
        {
            drawTime = 0;
            weapon.crosshair.SetActive(true);
            isAiming = true;
            _animator.ToggleAiming(isAiming);
            if (CurrentArrow == null)
            {
                PrepareNextArrow();
            }
        }

        protected override void StopAiming()
        {
            isAiming = false;
            _animator.ToggleAiming(isAiming);
            drawTime = 0f;
            weapon.crosshair.SetActive(false);
        }

     

        public override void OnEquip()
        {
            animator.DrawArrow(true);
        }

        public override void OnUnEquip()
        {
            animator.DrawArrow(false);
        }

        protected override void Shoot() // this might be a littile different 
        {
            if (CurrentArrow == null)
            {
                return;
            }

            _animator.Shoot();


            ArrowMove arrowMove = CurrentArrow.GetComponent<ArrowMove>();
            arrowMove.InitDamage(data.weaponSo.damage);
            if (CurrentArrow == null && !isAiming) return;

            float powerPercent = drawTime / maxDrawTime;
            float arrowSpeed = Mathf.Lerp(_mixArrowSpeed, _maxArrowSpeed, powerPercent);
            CurrentArrow.transform.SetParent(null);


            Vector3 shootDirection = (aimTarget.position - RestPoint.position).normalized;
            arrowMove.ShootArrow(shootDirection, arrowSpeed);
            _animator.RestShootable();

            CurrentArrow = null;
            drawTime = 0f;
            isAiming = false;
            animator.Aim(false);
        }
    }
}