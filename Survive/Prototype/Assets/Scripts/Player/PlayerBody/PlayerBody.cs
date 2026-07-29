using System;
using System.Collections;
using System.Collections.Generic;
using Effect;
using Effect.Symptoms;
using UnityEngine;

//ToDo : Fire damage and Poison damage is a shared Behaviour find solution for that 
public class PlayerBody : MonoBehaviour
{
    // this script will keep track of player body status  
    [SerializeField] private GameObject bodyParent;

    private bool _isWounded;
    private PlayerUI _playerUI;
    private PlayerVitalStats _playerVitalStats;
    private List<ActiveEffect> _activeEffects;

    private List<ActiveSymptom> _activeSymptoms;
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

    public void AddSymptom(Symptom symptom)
    {
    }

    public void TakeDamage(IAttack attack)
    {
        foreach (var effect in attack.Effects)
        {
            foreach (var type in effect.symptoms)
            {
                ActiveSymptom s = new ActiveSymptom();
                s.activeSymptom = CreateSymptom(type);
                s.Type = type;
                _activeSymptoms.Add(s);
            }

            ActiveEffect active = new ActiveEffect(effect);
            active.damageRoutine = StartCoroutine(HandleEffectDamage(active));

            _activeEffects.Add(active);
        }
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

    private IEnumerator HandleEffectDamage(ActiveEffect active)
    {
        EffectsSo data = active.data;

        while (active.elapsedTime < data.MaxTime)
        {
            // Wait for the time frame
            yield return new WaitForSeconds(data.timeFrame * 60f);

            // Apply damage
            //    _playerVitalStats.DecreaseHealth(data.damage);

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
            // 
            ActiveSymptom activeSymptom = _activeSymptoms.Find(s => s.Type == type);
            if (activeSymptom != null)
            {
                activeSymptom.activeSymptom.StopSymptom();
                _activeSymptoms.Remove(activeSymptom);
            }
        }

        _activeEffects.Remove(active);
    }
}

public class ActiveEffect
{
    public EffectsSo data;
    public float elapsedTime;
    public Coroutine damageRoutine;

    public ActiveEffect(EffectsSo data)
    {
        this.data = data;
        elapsedTime = 0f;
    }
}

public class ActiveSymptom
{
    public Symptom activeSymptom;
    public BaseSymptomType Type;
}