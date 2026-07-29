using Effect;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Bandage : BaseResource, IHeal // this is a resourse and an imventory intem 
{
    [SerializeField] private Button useMeButton;
    public EffectsSo EffectsSo { get; }

    protected override void Awake()
    {
        useMeButton.onClick.AddListener(HealPlayer);
        base.Awake();
    }

    private void UseMe()
    {
        PlayerRepository.instance.HealPlayer(EffectsSo);
        // Apply Bandage on Body UI
        // get Player body Reference 
    }


    public void HealPlayer()
    {
    }
}