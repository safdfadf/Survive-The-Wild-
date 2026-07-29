using System.Collections.Generic;
using Effect;
using UnityEngine;

[System.Serializable]
public class AnimalAttack: IAttack
{
    [SerializeField] private bool isFire;
    [SerializeField] private bool isPoison;
    [SerializeField] private bool isStun;
    [SerializeField] private int damage;
    
    [SerializeField] private List<EffectsSo> effects;
    public bool IsFire => isFire;
    public bool IsPoison => isPoison;
    public bool IsStun => isStun;
    public int Damage => damage;
    public List<EffectsSo> Effects => effects;
    // now we have effects which can regular damage , damage over time and apply effects according 
    // do i need these boolians? 
}
