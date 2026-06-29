using System;
using UnityEngine;

public class PlayerNoiseEmitter : MonoBehaviour
{
  public float walkNoise { get; private set; } = 1.5f;
    public float sprintNoise { get; private set; }= 2.5f;
    public float crouchNoise = 0.2f;
    private MovementHandler _player;

    [Header("Decay")]
    public float noiseDecayRate = 0.005f; // per second

    private float _uiNoiseValue = 0f;//for ui
    private float _sourceNoiseStrength = 0f;

    private void Awake()
    {
        _player = GetComponent<MovementHandler>();
    }

    private void Update()
    {
        DecayNoiseValues();
    }

    private void DecayNoiseValues()
    {
        _sourceNoiseStrength -= noiseDecayRate * Time.deltaTime;
        _sourceNoiseStrength = Mathf.Max(0,_sourceNoiseStrength);
        _uiNoiseValue -= noiseDecayRate * Time.deltaTime;
        _uiNoiseValue = Mathf.Clamp01(_uiNoiseValue);
    }
    public void AddNoise(float amount, bool isSprinting)
    {
        _sourceNoiseStrength =Mathf.Max(_sourceNoiseStrength,amount);
        _uiNoiseValue = amount;
    }
    public float GetCurrentNoise()
    {
        return _uiNoiseValue;
    }
    // Animals call this
    public float GetNoiseIntensityAt(Vector3 animalPos)
    {
        Vector3 a = new Vector3(animalPos.x, 0f, animalPos.z);
        Vector3 p = new Vector3(transform.position.x, 0f, transform.position.z);

        float dist = Vector3.Distance(a, p);
        dist = Mathf.Max(dist, 0.5f); // avoid extreme close spikes

        float intensity = _sourceNoiseStrength / dist; // inverse-linear
        return Mathf.Clamp01(intensity);
    }
}


