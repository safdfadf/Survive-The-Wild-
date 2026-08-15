using System;
using UnityEngine;

public class HitBox : MonoBehaviour, ItakeDamage
{
    private TargetPractice mainBody;
    private AnimalBase _animal;
    public bool IsEnvironment { get; set; }


    [SerializeField] private int damageMultiplayer;

    private void Awake()
    {
        _animal = GetComponentInParent<AnimalBase>();
    }

    public void Initialize(AnimalBase scheduledAnimal)
    {
        _animal = scheduledAnimal;
        if (_animal == null)
        {
            Debug.LogError(_animal.name + " is missing animal");
        }
    }


   
    public void TakeDamage(int damage, Vector3 contactPoint)
    {
        if (_animal != null)
            _animal.TakeDamage(damageMultiplayer, contactPoint);
    }
}