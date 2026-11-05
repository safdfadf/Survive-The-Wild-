using System;
using Player;
using UnityEngine;

// based on location damage will be reduced 
public class HitBox : MonoBehaviour, ItakeDamage
{
    [SerializeField] private DamageLocation dmgLoc;
    private TargetPractice mainBody;
    private AnimalBase _animal;
    private Collider _collider;
    public bool IsEnvironment { get; set; }


    [SerializeField] private int damageMultiplayer;

    private void Awake()
    {
        _animal = GetComponentInParent<AnimalBase>();
        _collider = GetComponent<Collider>();
    }

    public void Initialize(AnimalBase scheduledAnimal)
    {
        _animal = scheduledAnimal;
        if (_animal == null)
        {
            Debug.LogError(_animal.name + " is missing animal");
        }
    }

    public void TakeDamage(IAttack attack)
    {
        var atk = attack as PlayerAttack;
        atk.Damage *= damageMultiplayer;
        atk.HitLocation = dmgLoc;
        _animal.TakeDamage(atk);
    }

    public void ToggleCollider(bool toggle)
    {
        _collider.enabled = toggle;
    }
}

[System.Serializable]
public enum DamageLocation
{
    Head,
    BackBody,
    MiddleBody
}