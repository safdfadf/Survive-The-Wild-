using System;
using System.Collections;
using UnityEngine;

public class PuddleFx : MonoBehaviour
{
    private ParticleSystem _bloodFx;

    private void Awake()
    {
        _bloodFx = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public IEnumerator ActivateFx()
    {
        _bloodFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _bloodFx.Play();

        yield return new WaitForSeconds(0.5f);
        _bloodFx.Pause();
        yield return new WaitForSeconds(12);
        _bloodFx.Stop();
    }


   
}