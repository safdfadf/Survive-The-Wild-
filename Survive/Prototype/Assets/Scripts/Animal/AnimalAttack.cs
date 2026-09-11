using System.Collections.Generic;
using Effect;
using UnityEngine;

[System.Serializable]
public class AnimalAttack : IAttack
{
    [SerializeField] private int damage;

    [SerializeField] private EffectsSo effects;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public EffectsSo Effects => effects;
}