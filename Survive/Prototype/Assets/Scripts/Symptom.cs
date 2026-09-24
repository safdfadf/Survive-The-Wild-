using System;
using System.Collections;
using System.Collections.Generic;
using SplineMesh;
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

    [SerializeField] private GameObject Spline;

    [Header("Blood")] [SerializeField] private Material bloodSpineMat;
    [SerializeField] private GameObject bloodPuddle;

    [Header("Vomit")] [SerializeField] private Material vomitSpineMat;
    [SerializeField] private GameObject vomitPuddle;
    private MovementHandler player;
  

    private void Awake()
    {
        player = GetComponent<MovementHandler>();
    }

    private void LateUpdate()
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
    private void TriggerRandomSymptom()
    {
        int r = UnityEngine.Random.Range(0, 2); // 0 or 1

        if (r == 0)
        {
            StartCoroutine(Dizziness());
        }
        else
        {
            Vomit();
        }
    }
    public void TriggerFeverSymptom()
    {
    }

    private void TriggerPoisonSymptom()
    {
        TriggerRandomSymptom();
    }

    private void TriggerInfectionSymptom()
    {
       TriggerRandomSymptom();
    }

    public void Hallucination() // maybe
    {
        Debug.Log("Hallucination");
    }

    private void Vomit()
    {
        ExampleContortAlong s = Spline.GetComponentInChildren<ExampleContortAlong>();
        s.Initialize(vomitSpineMat, player);
        GameObject vp = Instantiate(vomitPuddle, Spline.transform.position, Quaternion.identity);
        vp.SetActive(false);
        WaterShaderFx fx = vomitPuddle.GetComponent<WaterShaderFx>();
        s.puddleFx = fx;
        player.SetBending(true);
        s.gameObject.SetActive(true);
        s.ActivateWaterMovement();
    }

    private void BloodVomit()
    {
        ExampleContortAlong s = Spline.GetComponentInChildren<ExampleContortAlong>();
        s.material = bloodSpineMat;
        WaterShaderFx fx = bloodPuddle.GetComponent<WaterShaderFx>();
        s.puddleFx = fx;
        s.ActivateWaterMovement();
    }

    private IEnumerator Dizziness()
    {
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