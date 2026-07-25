using System;
using UnityEngine;

public class Trees : Environment
{
    [SerializeField] private GameObject TestHitPoint;
    private void Awake()
    {
        ResourceDropCount = 3;
      
    }

    private void OnCollisionEnter(Collision other)
    {
        BaseWeapon weapon = other.gameObject.GetComponent<BaseWeapon>();
        if (weapon != null)
        {
            int damage = weapon.MaxDamage;
            Vector3 contactPoint = other.contacts[0].point;
            TakeDamage(damage,contactPoint);   
        }
    }
}
