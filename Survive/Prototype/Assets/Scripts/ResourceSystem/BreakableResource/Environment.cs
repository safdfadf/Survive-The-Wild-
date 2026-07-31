using System.Collections.Generic;
using System.Linq;
using FoodSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class Environment : MonoBehaviour, ItakeDamage, IsoInitializer<EnvironSo>
{
    [SerializeField] protected int resourceDropCount;
    [SerializeField] protected Vector3 dropOffset;
    public EnvironSo environSo { get; private set; }
    protected int currentHealth;
    protected PosInChunk cashedPosInChunk;
    [Header("Dropables")] [SerializeField] private List<FoodSo> foodSos;

    [SerializeField] private List<PosInEnvironment> pos;

    public void Initialize(EnvironSo so)
    {
        environSo = so;
        SpawnFood();
        if (environSo == null) return;
    }

    private void SpawnFood()
    {
        foreach (var so in foodSos)
        {
            for (int i = 0; i < so.amount; i++)
            {
                Vector3 pos = GetPosition();
                if (pos == Vector3.zero) continue;
                GameObject food = Instantiate(so.prefab, gameObject.transform, true);
                Food foodScript = food.GetComponent<Food>();
                foodScript.Initialize(so);
                food.transform.position = pos;
            }
        }
    }

    public void SeCashedPos(PosInChunk casedPos)
    {
        cashedPosInChunk = casedPos;
    }

    private void OnCollisionEnter(Collision other)
    {
        BaseWeapon weapon = other.gameObject.GetComponent<BaseWeapon>();
        if (weapon != null && environSo.canBreak)
        {
            int damage = weapon.MaxDamage; // test
            Vector3 contactPoint = other.contacts[0].point;
            TakeDamage(damage, contactPoint);
        }
    }

    public void TakeDamage(int damage, Vector3 contactPoint)
    {
        Debug.Log(gameObject.name + " taking damage " + damage);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private Vector3 GetPosition()
    {
        PosInEnvironment available = pos.FirstOrDefault(p => p.isAvailable);

        if (available != null)
        {
            available.isAvailable = false;
            return available.pos.position;
        }

        return Vector3.zero; // or handle "no available pos"
    }

    protected virtual void Break()
    {
        for (int i = 0; i < resourceDropCount; i++)
        {
            Debug.Log("i am breaking");
            // call spawner
            GameObject result = Instantiate(environSo.breakableData.resourceSo.prefab,
                transform.position + Random.insideUnitSphere * 0.5f + dropOffset, Quaternion.identity);
            BaseResource baseResource = result.GetComponent<BaseResource>();
            if (baseResource != null)
            {
                baseResource.Initialize(environSo.breakableData.resourceSo);
                Rigidbody rb = result.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                Collider collider = result.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = true;
                }
                else
                {
                    Debug.Log(result.name + " is missing Collider");
                }
            }
        }

        Destroy(gameObject);
    }
}

[System.Serializable]
public class PosInEnvironment
{
    public Transform pos;
    public bool isAvailable;
}