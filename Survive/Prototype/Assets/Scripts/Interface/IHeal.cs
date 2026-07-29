using Effect;
using UnityEngine;

public interface IHeal
{
    public EffectsSo EffectsSo { get; }
    public void HealPlayer();
}
