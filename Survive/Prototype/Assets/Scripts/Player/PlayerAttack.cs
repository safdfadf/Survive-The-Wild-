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

        [SerializeField] private List<EffectsSo> effects;// we can later add poison/Fire damage 
        
        public int Damage => damage;
        public List<EffectsSo> Effects => effects;
        public WeaponAbility ability;


    }
}