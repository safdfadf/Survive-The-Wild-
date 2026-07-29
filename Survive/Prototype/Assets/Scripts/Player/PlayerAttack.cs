using System.Collections.Generic;
using Effect;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerAttack : IAttack
    {
        [SerializeField] private int damage;

        [SerializeField] private List<EffectsSo> effects;

        public int Damage => damage;
        public List<EffectsSo> Effects => effects;
    }
}