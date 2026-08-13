using System;
using System.Collections;
using System.Collections.Generic;
using Effect;
using Effect.Symptoms;
using Player;
using UnityEngine;

//ToDo : Fire damage and Poison damage is a shared Behaviour find solution for that 
public class PlayerBody : MonoBehaviour
{
    // this script will keep track of player body status  
    [SerializeField] private PlayerAttack attackToSelf;
    private bool _isAbleToInfect;
    private PlayerUI _playerUI;
    private PlayerVitalStats _playerVitalStats;
    private List<ActiveEffect> _activeEffects= new();

    private List<ActiveSymptom> _activeSymptoms= new();

    // current issue regular damage : wound is created 
    private void Awake()
    {
        _playerUI = GetComponent<PlayerUI>();
        _playerVitalStats = GetComponent<PlayerVitalStats>();
    }


    public void HealPlayer(EffectsSo effectsSo)
    {
        foreach (var effect in _activeEffects)
        {
            if (!effect.data == effectsSo) continue;
            RemoveEffect(effect);
        }
    }
    public void TakeDamage(IAttack attack)
    {
        Debug.Log("did Damage");
        foreach (var effect in attack.Effects)
        {
            if (effect.symptoms.Contains(BaseSymptomType.None))
            {
                //    _playerVitalStats.DecreaseHealth(effect.damage);

                ActiveEffect woundEffect = new ActiveEffect(effect);
                woundEffect.woundTimerRoutine = StartCoroutine(HandleWoundTimer(woundEffect));

                _activeEffects.Add(woundEffect);
                ApplyWound(effect.woundMaterial);
                continue;
            }

            // 1. Apply symptoms (if any)
            foreach (var type in effect.symptoms)
            {
                if (type != BaseSymptomType.None)
                {
                    ActiveSymptom s = new ActiveSymptom();
                    s.activeSymptom = CreateSymptom(type);
                    s.Type = type;
                    _activeSymptoms.Add(s);
                }
            }

            ActiveEffect active = new ActiveEffect(effect);
            active.damageRoutine = StartCoroutine(HandleEffectDamage(active));
            _activeEffects.Add(active);
            ApplyWound(effect.woundMaterial);
            
        }
        
        Debug.Log(attack.Effects[0].name);
    }

    public void TakeRegularDamage(EffectsSo effectsSo) // should this be an effect aswell 
    {
        // playerVitalStats.DecreaseStats   
    }

    private Symptom CreateSymptom(BaseSymptomType type)
    {
        return type switch
        {
            BaseSymptomType.Dizziness => new DizzinessSymptom(this),
            BaseSymptomType.Vomit => new VomitSymptom(this),
            BaseSymptomType.Hallucination => new HallucinationSymptom(this),
            BaseSymptomType.Unconscious => new UnconsciousSymptom(this),
            _ => null
        };
    }

    private IEnumerator HandleWoundTimer(ActiveEffect wound)
    {
        float timer = wound.data.MaxTime * 60f;

        while (timer > 0f)
        {
            if (wound.isHealed)
                yield break;

            timer -= Time.deltaTime;
            yield return null;
        }

        ApplyInfectionEffect();
    }

    private void ApplyInfectionEffect() // infection is like an attack to self 
    {
        TakeDamage(attackToSelf);
    }

    private IEnumerator HandleEffectDamage(ActiveEffect active)
    {
        EffectsSo data = active.data;

        while (active.elapsedTime < data.MaxTime)
        {
            // Wait for the time frame
            yield return new WaitForSeconds(data.timeFrame * 60f);
            active.elapsedTime += data.timeFrame;
        }

        RemoveEffect(active);
    }

    private void RemoveEffect(ActiveEffect active)
    {
        if (active.damageRoutine != null)
            StopCoroutine(active.damageRoutine);
        foreach (var type in active.data.symptoms)
        {
            ActiveSymptom activeSymptom = _activeSymptoms.Find(s => s.Type == type);
            if (activeSymptom != null)
            {
                activeSymptom.activeSymptom.StopSymptom();
                _activeSymptoms.Remove(activeSymptom);
            }
        }

        _activeEffects.Remove(active);
    }

    private void ApplyWound(Material mat)// temp 
    {
        Debug.Log("Applying Wound");
     _playerUI.ApplyWoundUI(mat);    
    }

}

public class ActiveEffect
{
    public EffectsSo data;
    public float elapsedTime;
    public Coroutine damageRoutine;
    public Coroutine woundTimerRoutine;
    public bool isHealed;

    public ActiveEffect(EffectsSo data)
    {
        this.data = data;
        elapsedTime = 0f;
        isHealed = false;
    }
}

public class ActiveSymptom
{
    public Symptom activeSymptom;
    public BaseSymptomType Type;
}