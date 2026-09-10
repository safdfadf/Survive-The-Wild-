using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace.Weapon
{
    // how do we control animator as we switch through weapon  
    public class WeaponBehaviour : MonoBehaviour
    {
        protected BaseWeapon weapon;
        protected PlayerAnimator animator;
        protected Transform aimTarget;

        protected WeaponData data;
      
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

        public virtual void OnUnEquip()
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