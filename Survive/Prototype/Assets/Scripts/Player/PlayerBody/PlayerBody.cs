using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Effect;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] private GameObject woundPrefab;
    [SerializeField] private Transform woundTransform;

    [FormerlySerializedAs("InfectionEffect")] [SerializeField]
    private DOtEffects BleedingEffect;

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
            if (CheckForActiveEffects(dot, out ActiveEffect existing))
            {
                existing.elapsedTime = 0; // reset
                // display time as well 
                return;
            }

            ActiveEffect activeEffect = new ActiveEffect(dot);
            _activeEffects.Add(activeEffect);
            StopCoroutine(HandleEffectDamage(activeEffect));
        }
        else // Regular damage 
        {
            Debug.Log("taking damage" + BleedingEffect);
            if (Random.value <= attack.BleedingProbab)
            {
                Debug.Log("Bleeding effect");
                // this comes true we c
                ActiveEffect woundEffect = new ActiveEffect(BleedingEffect);
                woundEffect.woundTimerRoutine = StartCoroutine(HandleEffectDamage(woundEffect));
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
        float timer = 5f;//wound.data.MaxTime * 60f;

        while (timer > 0f)
        {
            if (wound.isHealed)
                yield break;
            timer -= Time.deltaTime;
            yield return null;
        }
        //ToDo: Infection
        // after this bleeding will stop and now if player has used bandage there will be less probabiliy pf infection 
        // if not there will be more chances and based on that infection will apppied 
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
        WoundUI ui = _playerUI.SetWoundUI(data);
        active.WoundUI = ui;
        while (active.elapsedTime < data.MaxTime)
        {
            yield return new WaitForSeconds(data.timeFrame * 60f);
            active.elapsedTime += data.timeFrame;
            ui.UpdateSlider(active.elapsedTime, data.MaxTime);
            // trigger symptom 
            // stamina depletion 
        }

        RemoveEffect(active);
    }

    private void RemoveEffect(ActiveEffect active)
    {
        if (active.damageRoutine != null)
            StopCoroutine(active.damageRoutine);

        Destroy(active.WoundUI);
        _activeEffects.Remove(active);
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