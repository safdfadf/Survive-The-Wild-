using System;
using UnityEngine;

public class Hatchet : BaseWeapon // hatchet does not needs to know who it is hitting 
{ 
    protected override void Awake()
    {
        inventoryRotAngle = 0;
        isAimable = false;
        isLeftHanded = false;
        Gm = gameObject;
        base.Awake();
    }
    protected override void Attack()
    {
         animator.SwordAttack();
         StartCoroutine(StartAttacking());
    }
    protected override void Block(int damage)
    {
        animator.SwordBlock();
    }
    private void OnTriggerEnter(Collider other)
    {
        ItakeDamage target = other.gameObject.GetComponent<ItakeDamage>();
        if (target != null&& player.isAttacking)
        {
            target.TakeDamage(CraftingSo.maxDamage);
        }
    }
    protected override void EquipMe()
    {
        animator.SwordEquipped(true);
        base.EquipMe();
    }
}
