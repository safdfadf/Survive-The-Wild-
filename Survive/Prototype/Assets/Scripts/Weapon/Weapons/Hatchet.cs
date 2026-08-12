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
        if (target != null && player.isAttacking)
        {
            Vector3 contactPoint = other.gameObject.transform.position;
            //    target.TakeDamage(CraftingSo.maxDamage,contactPoint);
        }
    }

    public override void UseMe()
    {
        base.UseMe();
        animator.SwordEquipped(true);
       
    }
}