using System;
using UnityEngine;

public class HitBox : MonoBehaviour, ItakeDamage
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


    public void TakeDamage(int damage, Vector3 contactPoint)
    {
        if (animal != null)
            animal.TakeDamage(damageMultiplayer, contactPoint);
    }
}