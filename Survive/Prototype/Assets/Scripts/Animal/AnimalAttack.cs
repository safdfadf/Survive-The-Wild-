using System.Collections.Generic;
using Effect;
using UnityEngine;

[System.Serializable]
public class AnimalAttack : IAttack
{
    [SerializeField] private int damage;

    [SerializeField] private List<EffectsSo> effects;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public List<EffectsSo> Effects => effects;
}