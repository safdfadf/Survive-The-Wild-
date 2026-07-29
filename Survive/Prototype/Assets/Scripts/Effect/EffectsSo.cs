using System.Collections.Generic;
using Effect.Symptoms;
using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "EffectSo", menuName = "Scriptable Objects/EffectSo")]
    public class EffectsSo : ScriptableObject
    {
        public List<BaseSymptomType> symptoms;
        public float damage;
        public float timeFrame;
        public float MaxTime;
        // wound type 
    }
}