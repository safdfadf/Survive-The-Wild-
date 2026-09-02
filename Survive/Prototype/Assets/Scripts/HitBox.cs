using System;
using Player;
using UnityEngine;

public class HitBox : MonoBehaviour, ItakeDamage
{
    private TargetPractice mainBody;
    private AnimalBase _animal;
    public bool IsEnvironment { get; set; }
    public void TakeDamage(IAttack attack)
    {
        _animal.TakeDamage(attack as PlayerAttack);
    }


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

}