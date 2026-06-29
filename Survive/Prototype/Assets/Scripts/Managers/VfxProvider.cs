using System;
using UnityEngine;
using UnityEngine.Serialization;

public class VfxProvider : MonoBehaviour
{
    public static VfxProvider Instance;
    [SerializeField] private GameObject bloodParticle;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetBloodParticle()
    {
        return bloodParticle;
    }
}
