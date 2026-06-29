using System;
using UnityEngine;

public class HitBox : MonoBehaviour,IArrowStickable
{
    private TargetPractice mainBody;
    private AnimalBase animal;

   [SerializeField] private int damageMultiplayer;
    private void Awake()
    {
        animal = GetComponentInParent<AnimalBase>();
       
    }
    public void Initialize(AnimalBase animal)
    {
        this.animal = animal;
        if (this.animal == null)
        {
            Debug.LogError(this.animal.name + " is missing animal");
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
        arrow.transform.position = point + offset;
        arrow.transform.rotation = Quaternion.LookRotation(-normal);
        arrow.transform.position += arrow.transform.forward * -0.05f;
        arrow.transform.SetParent(transform, true);
    }
    public void TakeDamage(Vector3 contact)
    {
        if (animal != null)
            animal.TakeDamage(damageMultiplayer,contact);
    }
}
