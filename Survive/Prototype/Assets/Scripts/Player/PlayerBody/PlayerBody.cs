using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Effect;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private GameObject woundPrefab;
    [SerializeField] private Transform woundTransform;
    [SerializeField] private DOtEffects InfectionEffect;
    private bool _isAbleToInfect;
    private PlayerUI _playerUI;
    private PlayerVitalStats _playerVitalStats;
    private List<ActiveEffect> _activeEffects = new();
    private Symptom _symptom;
    private List<ActiveSymptom> _activeSymptoms = new();

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
        _playerVitalStats.DamageToHealth(attack.Damage);

        DOtEffects dot = attack.Effects as DOtEffects;
        if (dot != null)
        {
            if (CheckForActiveEffects(dot,out ActiveEffect existing))
            {
                // increase elapsed time 
                existing.elapsedTime = 0;// reset
                return;
            }

            ActiveEffect activeEffect = new ActiveEffect(dot);
            _activeEffects.Add(activeEffect);
            StopCoroutine(HandleEffectDamage(activeEffect));
        }
        else
        {
            if (Random.value <= attack.Effects.InfectionChance)
            {
                ActiveEffect woundEffect = new ActiveEffect(InfectionEffect);
                woundEffect.woundTimerRoutine = StartCoroutine(HandleWoundTimer(woundEffect));
                _activeEffects.Add(woundEffect);
            }
        }
    }

    private bool CheckForActiveEffects(DOtEffects newDot, out ActiveEffect effect)
    {
        foreach (var e in _activeEffects)
        {
            if (e.data == newDot)
            {
                effect = e;
                return true;
            }
        }
        effect = null;
        return false;
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
        ApplyInfectionEffect(wound);

        yield return null; // remove this
    }

    private void ApplyInfectionEffect(ActiveEffect effect)
    {
        SelfAttack atk = new SelfAttack(0, Vector3.zero);
        atk.Effects = effect.data;
        TakeDamage(atk);
    }

    private IEnumerator HandleEffectDamage(ActiveEffect active)
    {
        DOtEffects data = active.data;

        while (active.elapsedTime < data.MaxTime)
        {
            yield return new WaitForSeconds(data.timeFrame * 60f);
            active.elapsedTime += data.timeFrame;
            // trigger symptom 
            // do damage over time if needed 
            // stamina depletion 
        }

        RemoveEffect(active);
    }

    private void RemoveEffect(ActiveEffect active)
    {
        if (active.damageRoutine != null)
            StopCoroutine(active.damageRoutine);
        RemoveWound();
        Destroy(active.WoundUI);
        _activeEffects.Remove(active);
    }

    private void RemoveWound()
    {
        Debug.Log("remove wound");
        _playerUI.ApplyOriginalUI();
    }
}

public class ActiveEffect
{
    public DOtEffects data;
    public float elapsedTime;
    public Coroutine damageRoutine;
    public Coroutine woundTimerRoutine;
    public bool isHealed;
    public WoundUI WoundUI;

    public ActiveEffect(DOtEffects data)
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