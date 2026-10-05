using Effect;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Bandage : Obj<ObjSo>, IHeal
{
    [SerializeField] private EffectsSo effectsSo;
    public EffectsSo EffectsSo { get; set; }

    protected override void Awake()
    {
        EffectsSo = effectsSo;
        canUse = true;
        base.Awake();
    }

    public override void UseMe()
    {
        PlayerRepository.instance.HealPlayer(EffectsSo);
        base.UseMe();
    }
    protected override void SetUiBools()
    {
        canCraft = true;
        canHarvest = false;
        canUse = true;
    }
}