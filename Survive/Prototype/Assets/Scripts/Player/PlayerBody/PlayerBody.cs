using System;
using System.Collections;
using System.Collections.Generic;
using Effect;
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
    private List<ActiveEffect> _activeEffects = new();
    private Symptom _symptom;
    private List<ActiveSymptom> _activeSymptoms = new();

    // current issue regular damage : wound is created 
    private void Awake()
    {
        _playerUI = GetComponent<PlayerUI>();
        _playerVitalStats = GetComponent<PlayerVitalStats>();
    }


    public void HealPlayer(EffectsSo effectsSo)
    {
        Debug.Log("heal player");
        foreach (var effect in _activeEffects)
        {
            if (!effect.data == effectsSo) continue;
            RemoveEffect(effect);
        }
    }

    public void TakeDamage(IAttack attack)
    {
        foreach (var effect in attack.Effects)
        {
            if (effect.damageType == DamageType.Regular)
            {
                //    _playerVitalStats.DecreaseHealth(effect.damage);
                Debug.Log("taking regular damage");
                ActiveEffect woundEffect = new ActiveEffect(effect);
                woundEffect.woundTimerRoutine = StartCoroutine(HandleWoundTimer(woundEffect));

                _activeEffects.Add(woundEffect);
                ApplyWound(effect.woundMaterial);
                continue;
            }

            ActiveEffect active = new ActiveEffect(effect);
            active.damageRoutine = StartCoroutine(HandleEffectDamage(active));
            _activeEffects.Add(active);
            ApplyWound(effect.woundMaterial);
        }

        Debug.Log(attack.Effects[0].name);
    }

    private void CreateSymptom(DamageType type) // here we dont need symptom but based on 
    {
        /*
        return type switch
        {
            BaseSymptomType.Dizziness => new DizzinessSymptom(this),
            BaseSymptomType.Vomit => new VomitSymptom(this),
            BaseSymptomType.Hallucination => new HallucinationSymptom(this),
            BaseSymptomType.Unconscious => new UnconsciousSymptom(this),
            _ => null
        };*/
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
        _symptom.ExecuteSympton(_activeEffects[0]);
        ApplyInfectionEffect();
    }

    private void ApplyInfectionEffect() // infection is like an attack to self 
    {
        Debug.Log("Applying infection effect");
        TakeDamage(attackToSelf);
    }

    private IEnumerator HandleEffectDamage(ActiveEffect active)
    {
        EffectsSo data = active.data;

        while (active.elapsedTime < data.MaxTime)
        {
           
            yield return new WaitForSeconds(data.timeFrame * 60f);
            active.elapsedTime += data.timeFrame;
        }
        
        RemoveEffect(active);
    }

    private void RemoveEffect(ActiveEffect active)
    {
        if (active.damageRoutine != null)
            StopCoroutine(active.damageRoutine);
        RemoveWound();
        _activeEffects.Remove(active);
    }

    private void ApplyWound(Material mat) // temp 
    {
        Debug.Log("Applying Wound");
        _playerUI.ApplyWoundUI(mat);
    }

    private void RemoveWound()
    {
        Debug.Log("remove wound");
        _playerUI.ApplyOriginalUI();
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
    public DamageType Type;
}