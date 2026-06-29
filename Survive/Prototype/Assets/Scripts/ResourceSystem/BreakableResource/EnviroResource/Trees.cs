using System;
using UnityEngine;

public class Trees : Environment,IArrowStickable
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
            Debug.Log("taking damage");
            int damage = weapon.MaxDamage;
         base.TakeDamage(damage);   
        }
    }
    public void StickArrow(GameObject arrow, Vector3 point, Vector3 offset, Vector3 normal)
    {
        var rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        arrow.transform.position = point;
        Instantiate(TestHitPoint, point, Quaternion.identity);
       // arrow.transform.rotation = Quaternion.LookRotation(-normal);
        arrow.transform.position += arrow.transform.forward * -0.05f;

        arrow.transform.SetParent(transform, true);
    }


    public void TakeDamage(Vector3 contact)
    {
       
    }
}
