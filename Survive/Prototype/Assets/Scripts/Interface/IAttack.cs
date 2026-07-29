using System.Collections.Generic;
using Effect;
using UnityEngine;

public interface IAttack
{
    public int Damage { get; }
    public List<EffectsSo> Effects { get; }
}