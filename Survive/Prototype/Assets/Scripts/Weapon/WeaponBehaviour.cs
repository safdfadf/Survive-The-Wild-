using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace.Weapon
{
    public class WeaponBehaviour : MonoBehaviour
    {
        protected BaseWeapon weapon;
        protected PlayerAnimator animator;
        protected Transform aimTarget;

        protected WeaponData data;
        [HideInInspector] public bool isEquipped;
        protected Transform cameraTransform;

        public virtual void Initialize(WeaponData data, PlayerAnimator animator, BaseWeapon weapon)
        {
            this.animator = animator;
            this.data = data;
            this.weapon = weapon;
            aimTarget = data.aimTarget;
            cameraTransform = Camera.main.transform;
        }

        public virtual void OnEquip()
        {
        }

        protected virtual void Attack()
        {
        }

        protected virtual void Block(int damage)
        {
        }

        public virtual void OnInput(InputAction.CallbackContext ctx)
        {
        }
        public virtual void DeliverDamage(){}
    }
}