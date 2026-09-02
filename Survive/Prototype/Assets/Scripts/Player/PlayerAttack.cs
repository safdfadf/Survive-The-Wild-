using System.Collections.Generic;
using DefaultNamespace.Weapon;
using Effect;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerAttack : IAttack
    {
        [SerializeField] private int damage;

        [SerializeField] private List<EffectsSo> effects; // we can later add poison/Fire damage 

        public int Damage
        {
            get => damage;
            set => damage = value;
        }

        public List<EffectsSo> Effects => effects;
        public WeaponAbility ability;
        public Vector3 hitPoint;

        public PlayerAttack(int damage, List<EffectsSo> effects, WeaponAbility ability, Vector3 hitLoc)
        {
            Damage = damage;
            this.effects = effects;
            this.ability = ability;
            hitPoint = hitLoc;
        }
    }
}