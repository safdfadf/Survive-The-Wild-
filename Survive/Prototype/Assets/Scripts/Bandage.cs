using Effect;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Bandage : BaseObj, IHeal // this is a resourse and an imventory intem 
{
    [SerializeField] private Button useMeButton;
    public EffectsSo EffectsSo { get; }

    protected override void Awake()
    {
        canUseButton = true;
        base.Awake();
    }

    public override void UseMe()
    {
        PlayerRepository.instance.HealPlayer(EffectsSo);
        // Apply Bandage on Body UI
        // get Player body Reference 
    }
    public void HealPlayer()
    {
    }
}