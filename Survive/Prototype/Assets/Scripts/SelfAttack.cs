using System.Collections.Generic;
using Effect;
using UnityEngine;

namespace DefaultNamespace
{
    public class SelfAttack : IAttack
    {
        public int Damage { get; set; }
        public List<EffectsSo> Effects { get; }
        public Vector3 HitPoint { get; set; }

        public SelfAttack(int damage, Vector3 hitPoint)
        {
          Damage = damage;
          HitPoint = hitPoint;
            
        }
    }
}