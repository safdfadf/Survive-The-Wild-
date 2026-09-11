using System.Collections.Generic;
using Effect;
using UnityEngine;

[System.Serializable]
public class AnimalAttack : IAttack
{
    [SerializeField] private int damage;

    [SerializeField] private EffectsSo effects;
    [SerializeField] private float bleedingProbability;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

   public float BleedingProbab=>bleedingProbability;
    public EffectsSo Effects => effects;
}