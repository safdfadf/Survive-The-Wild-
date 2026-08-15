using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Symptom : MonoBehaviour
{
    [Header("Blur")] [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float maxScale;
    [SerializeField] private float holdTime;

    [FormerlySerializedAs("_blurShader")] [SerializeField]
    private Material blurShader;

    [SerializeField] private GameObject blurGameObject;

    private void Awake()
    {
   
    }

    public void ExecuteSympton(ActiveEffect activeEffect)
    {
        switch (activeEffect.data.damageType)
        {
            case DamageType.Poison:
                TriggerPoisonSymptom();
                break;
            case DamageType.Infection:
                TriggerInfectionSymptom();
                break;
        }
    }

    public void TriggerFeverSymptom()
    {
    }

    private void TriggerPoisonSymptom()
    {
    }

    public void TriggerInfectionSymptom()
    {
    }

    public void Hallucination() // maybe
    {
        Debug.Log("Hallucination");
    }

    private void Vomit()
    {
        Debug.Log("Vomit");
        // trigger audio 
        // trigger Anim
        // trigger Vomit fx 
    }

    private void BloodVomit()
    {
    }

    private IEnumerator Dizziness()
    {
        Debug.Log("Dizziness");
        blurGameObject.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            float t = 0f;

            while (t < fadeInTime)
            {
                t += Time.deltaTime;
                float percent = t / fadeInTime;
                blurShader.SetFloat("_Scale", Mathf.Lerp(0f, maxScale, percent));
                yield return null;
            }

            // Hold max blur
            blurShader.SetFloat("_Scale", maxScale);
            yield return new WaitForSeconds(holdTime);

            // Fade-out: maxScale → 0
            t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.deltaTime;
                float percent = t / fadeOutTime;
                blurShader.SetFloat("_Scale", Mathf.Lerp(maxScale, 0f, percent));
                yield return null;
            }

            blurShader.SetFloat("_Scale", 0f);
        }

        blurGameObject.SetActive(false);
    }
}

[System.Serializable]
public enum DamageType
{
    Regular,
    Poison,
    Infection
}