using System;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    // this script will keep track of player body status  

    [SerializeField] public bool isInjured { get; set; }
    public bool isPoisoned;
    public bool isInfected;
    public bool isOnFire;

    private PlayerUI _playerUI;
    private PlayerVitalStats _playerVitalStats;

    private void Awake()
    {
        _playerUI = GetComponent<PlayerUI>();
        _playerVitalStats = GetComponent<PlayerVitalStats>();
    }

    public void HealPlayer()
    {
    }

    public void HealPoison()
    {
    }

    public void SpreadPoison()
    {
    }

    public void SpreadInfection()
    {
    }

    public void ApplyDamageToLimb()
    {
        isInjured = true;
    }
}