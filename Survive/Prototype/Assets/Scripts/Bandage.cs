using Effect;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Bandage : BaseObj, IHeal
{
    [SerializeField] private Button useMeButton;
    [SerializeField] private EffectsSo effectsSo;
    public EffectsSo EffectsSo { get; set; } 
    protected override void Awake()
    {
        EffectsSo = effectsSo;
        canUseButton = true;
        base.Awake();
    }

    public override void UseMe()
    {
        PlayerRepository.instance.HealPlayer(EffectsSo);
    }
    public void HealPlayer()
    {
    }
}