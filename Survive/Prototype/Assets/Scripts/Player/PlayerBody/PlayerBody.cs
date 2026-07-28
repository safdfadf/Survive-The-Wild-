using System;
using System.Collections.Generic;
using UnityEngine;

//ToDo : Fire damage and Poison damage is a shared Behaviour find solution for that 
public class PlayerBody : MonoBehaviour
{
    // this script will keep track of player body status  
    [SerializeField] private GameObject bodyParent;

    public bool isPoisoned;
    public bool isInfected;
    public bool isOnFire;
    private bool _isWounded;
    private PlayerUI _playerUI;
    private PlayerVitalStats _playerVitalStats;

    [Header("Poison Effect")] [SerializeField]
    private float poisonDamagePerHour = 5f;

    [SerializeField] private int maxPoisonHours = 12;

    private int poisonHoursPassed = 0;

    private void Awake()
    {
        _playerUI = GetComponent<PlayerUI>();
        _playerVitalStats = GetComponent<PlayerVitalStats>();
    }

    private void OnEnable()
    {
        EventBus.OnHourChanged += HandlePoisonDamage;
    }

    private void OnDisable()
    {
        EventBus.OnHourChanged -= HandlePoisonDamage;
    }

    public void HealPlayer()
    {
    }

    public void HealPoison()
    {
    }

    public void ApplyPoison()
    {
    }

    public void SpreadInfection()
    {
    }

    private void ApplyDamage()
    {
        if(isPoisoned)
            HandlePoisonDamage(-1);
    }

    private void HandlePoisonDamage(int hour)
    {
        if (!isPoisoned)
            return;

        poisonHoursPassed++;

        _playerVitalStats.DamageToHealth(poisonDamagePerHour);

        if (poisonHoursPassed >= maxPoisonHours)
        {
            Debug.Log("Player died from poison.");
            _playerVitalStats.KillPlayer();
        }
        // add further effects like Vomit/Dizziness
        // and effect on Body UI 
    }

    public void TakeDamage(IAttack attack)
    {
        isPoisoned = attack.IsPoison;
        isOnFire = attack.IsFire;
        ApplyDamage();
    }
}