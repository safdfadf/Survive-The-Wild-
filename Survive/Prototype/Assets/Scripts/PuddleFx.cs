using System;
using System.Collections;
using UnityEngine;

public class PuddleFx : MonoBehaviour
{
    private ParticleSystem[] _waterFx;

    private void Awake()
    {
        _waterFx = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    public IEnumerator ActivateFx()
    {
        foreach (var fx in _waterFx)
        {
            fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fx.Play();
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void PauseFx()
    {
        _waterFx[0].Pause();
    }

    public IEnumerator DeactivateFx()
    {
        yield return new WaitForSeconds(2f);
        var renderer = _waterFx[0].GetComponent<ParticleSystemRenderer>();
        Material mat = renderer.material;

        mat.SetFloat("_Alpha", 0f);
        yield return new WaitForSeconds(2f);
        // gameObject.SetActive(false);
    }
}