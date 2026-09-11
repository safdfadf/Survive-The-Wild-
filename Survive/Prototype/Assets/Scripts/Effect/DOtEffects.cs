using UnityEngine;

namespace Effect
{
    [CreateAssetMenu(fileName = "EffectSo", menuName = "Scriptable Objects/Effects/DotSo", order = 0)]
    public class DOtEffects : EffectsSo
    {
        public float damageOverTime;
        public float timeFrame;
        public float MaxTime;
    }
}