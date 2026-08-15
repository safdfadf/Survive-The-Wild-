using System;
using UnityEngine;

public class Hatchet : BaseWeapon // hatchet does not needs to know who it is hitting 
{
    private ItakeDamage _currentTarget;

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
        animator.SwordAttack(); // make it a trigger 
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
        }
    }

    public override void UseMe()
    {
        base.UseMe();
        animator.SwordEquipped(true);
    }
}