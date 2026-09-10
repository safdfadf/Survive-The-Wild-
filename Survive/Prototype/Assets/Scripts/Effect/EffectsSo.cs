using System.Collections.Generic;

using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "EffectSo", menuName = "Scriptable Objects/Effects/EffectSo")]
    public class EffectsSo : ScriptableObject
    {
        public DamageType damageType;
        public float damage;
        public float timeFrame;
        public float MaxTime;
        public string description;
        public Sprite icon;
        public Material woundMaterial;// temp
       
    }
}