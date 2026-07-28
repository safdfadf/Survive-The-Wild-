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

    public void ApplyPoison()
    {
    }

    public void SpreadInfection()
    {
    }

    private void ApplyDamage()
    {
        
    }

    public void TakeDamage(IAttack attack)
    {
        isPoisoned = attack.IsPoison;
        isOnFire = attack.IsFire;
        ApplyDamage();
    }   
  
}