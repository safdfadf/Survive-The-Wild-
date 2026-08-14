using System.Collections.Generic;

using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "EffectSo", menuName = "Scriptable Objects/EffectSo")]
    public class EffectsSo : ScriptableObject
    {
        public DamageType damageType;
        public float damage;
        public float timeFrame;
        public float MaxTime;
        public Material woundMaterial;// temp
       
    }
}