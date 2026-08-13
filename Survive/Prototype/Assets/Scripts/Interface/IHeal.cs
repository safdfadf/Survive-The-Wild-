using Effect;
using UnityEngine;

public interface IHeal
{
    public EffectsSo EffectsSo { get;  set; }
    public void HealPlayer();
}
