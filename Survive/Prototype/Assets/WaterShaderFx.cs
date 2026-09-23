using System;
using UnityEngine;

public class WaterShaderFx : MonoBehaviour
{
    private ParticleSystem[] _waterFx;

    private void Awake()
    {
        _waterFx = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    public void ActivateFx()
    {
        gameObject.SetActive(true);
        foreach (var fx in _waterFx)
        {
            fx.Play();
        }
    }

    public void PauseFx()
    {
        _waterFx[0].Pause();
    }

    public void DeactivateFx()
    {
        gameObject.SetActive(false);
    }
}
