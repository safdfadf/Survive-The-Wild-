using UnityEngine;

public class Symptom : MonoBehaviour
{
    
    public void ExecuteSympton(ActiveEffect activeEffect)
    {
        switch ( activeEffect.data.damageType)
        {
            case DamageType.Poison: TriggerPoisonSymptom();
                break;
            case DamageType.Infection: TriggerInfectionSymptom();
                break;
        }
     
    }
    public void TriggerFeverSymptom()
    {
        
    }

    public void TriggerPoisonSymptom()
    {
        
    }

    public void TriggerInfectionSymptom()
    {
        
    }
    public void Hallucination()
    {
        
    }

    private void Vomit()
    {
        // trigger audio 
        // trigger Anim
        // trigger Vomit fx 
    }

    private void Dizziness()
    {
        // trigger audio 
        
    }
}

[System.Serializable]
public enum DamageType
{
    Regular,Poison,Infection
}