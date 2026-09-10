using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.Universal;

public class TakeDamageEffect : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private Vignette vignette;


    private void Start()
    {
        volume.profile.TryGet(out vignette);
    }

    public void TriggerDamageEffect() // when player health is decreasing trigger this 
    {
        StopAllCoroutines();
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        float intensity = 0.45f;
        vignette.intensity.value = intensity;

        yield return new WaitForSeconds(0.1f);

        while (intensity > 0)
        {
            intensity -= Time.deltaTime * 1.5f;
            vignette.intensity.value = intensity;
            yield return null;
        }

        vignette.intensity.value = 0;
    }
}