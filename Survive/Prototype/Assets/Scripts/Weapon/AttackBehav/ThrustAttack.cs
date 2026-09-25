using System;
using System.Collections;
using DefaultNamespace.Weapon.WeaponAnims;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DefaultNamespace.Weapon
{
    public class ThrustAttack : WeaponBehaviour
    {
        private ThrustAnims _thrustAnims;

        private void Awake()
        {
            _thrustAnims = GetComponent<ThrustAnims>();
        }

        private void LateUpdate()
        {
            Player.SetSpineControl(true);
        }

        public override void OnInput(InputAction.CallbackContext ctx)
        {
            if (weapon.isEquipped || weapon.RestrictUse) return;
            switch (ctx.interaction)
            {
                case TapInteraction:
                    Attack();
                    break;
            }
        }

        protected override void Attack()
        {
            _thrustAnims.ThrustAnim();
            StartCoroutine(StartAttacking());
        }

        private IEnumerator StartAttacking()
        {
            PlayerRepository.instance.SetAttacking(true); // what was this for ?
            yield return new WaitForSeconds(1f);
            PlayerRepository.instance.SetAttacking(true);
        }

        public override void OnEquip()
        {
            animator.HandWeaponEquip(true);
        }

        public override void OnUnEquip()
        {
            animator.HandWeaponEquip(false);
        }
    }
}