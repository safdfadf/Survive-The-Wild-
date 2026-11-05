using System.Collections.Generic;
using SurviveTheWild.Weapon;
using Effect;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [System.Serializable]
    public class PlayerAttack : IAttack
    {
        [SerializeField] private int damage;

        [SerializeField] private EffectsSo effects; // we can later add poison/Fire damage 

        public int Damage
        {
            get => damage;
            set => damage = value;
        }

        public EffectsSo Effects => effects;
        public WeaponAbility ability;
        [FormerlySerializedAs("hitPoint")] public DamageLocation HitLocation;
        public float BleedingProbab { get; set; }
        public PlayerAttack(int damage, EffectsSo effects, WeaponAbility ability)
        {
            Damage = damage;
            this.effects = effects;
            this.ability = ability;
           
        }
    }
}


// if damage is on head its obviously deadly and animal will drop directly , i damage to the back/middle  animal will run away if it is in the middle 
// damage is deadly run and drop 