using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace DefaultNamespace.Weapon
{
    public class SwingAttack : WeaponBehaviour
    {
        private bool isHolding;

        protected override void Attack()
        {
            if(isHolding)return;
            animator.SwordAttack(); //ToDo: make it a trigger 
            StartCoroutine(StartAttacking());
        }

        public override void OnInput(InputAction.CallbackContext ctx)
        {
            if (!isEquipped) return;
            switch (ctx.interaction)
            {
                case TapInteraction:
                case HoldInteraction when ctx.phase == InputActionPhase.Performed:
                    Attack();
                    isHolding = true;
                    break;
                case HoldInteraction when ctx.phase == InputActionPhase.Canceled:
                    isHolding = false;
                    break;
            }
        }

        protected override void Block(int damage)
        {
            animator.SwordBlock();
        }

        public override void OnEquip()
        {
            animator.HandWeaponEquip(true);
          
        }

        protected virtual GameObject IsInRange()
        {
            float radius = .8f;
            float maxAngle = 60f;
            Transform t = PlayerRepository.instance.GetPlayerTransform();
            Collider[] hits = Physics.OverlapSphere(t.position, radius);

            foreach (Collider col in hits)
            {
                if (col.TryGetComponent<ItakeDamage>(out var damageable))
                {
                    Vector3 dirToTarget = (col.transform.position - t.position).normalized;
                    float angle = Vector3.Angle(t.forward, dirToTarget);

                    if (angle <= maxAngle)
                    {
                        return col.gameObject;
                    }
                }
            }

            return null;
        }

        public override void DeliverDamage()
        {
            GameObject obj = IsInRange();
            if (obj == null) return;
            ItakeDamage dmgObj = obj.GetComponent<ItakeDamage>();
            if (dmgObj != null)
            {
                PlayerAttack attack = new PlayerAttack(data.weaponSo.damage,null, weapon.Ability,Vector3.zero);
                dmgObj.TakeDamage(attack);
            }
        }

        private IEnumerator StartAttacking()
        {
            PlayerRepository.instance.SetAttacking(true);
            yield return new WaitForSeconds(1f);
            PlayerRepository.instance.SetAttacking(true);
        }
    }
}