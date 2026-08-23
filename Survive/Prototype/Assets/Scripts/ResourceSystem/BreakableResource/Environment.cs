using System;
using System.Collections.Generic;
using System.Linq;
using FoodSystem;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//ToDo: add growth script 
public class Environment : MonoBehaviour, ItakeDamage, IsoInitializer<EnvironSo>
{
    [SerializeField] protected int resourceDropCount;
    [SerializeField] protected Vector3 dropOffset;

    public EnvironSo environSo { get; private set; }
    protected int currentHealth;
    protected PosInChunk cashedPosInChunk;
    [Header("Dropables")] [SerializeField] private List<FoodSo> foodSos;

    [SerializeField] private List<PosInEnvironment> pos;

    [FormerlySerializedAs("_damagedVersions")] [Header("DamagedVersions")] [SerializeField]
    protected List<GameObject> damagedVersions;

    [Header("Damage Threshold")] [SerializeField]
    private int damageStage1 = 25;

    [SerializeField] private int damageStage2 = 60;
    [SerializeField] private int damageStage3 = 90;
    private LODGroup _lodGroup;
    public bool IsEnvironment { get; set; }
    public bool IsPlayerInRange { get; set; }


    protected virtual void Awake()
    {
        _lodGroup = GetComponent<LODGroup>();
    }

    public void Initialize(EnvironSo so)
    {
        environSo = so;
        IsEnvironment = true;
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

    private void OnTriggerEnter(Collider other)
    {
        PlayerInRange(true);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInRange(false);
    }

    protected virtual void PlayerInRange(bool inRange)
    {
        IsPlayerInRange = inRange;
    }


    public void TakeDamage(int damage, Vector3 contactPoint)
    {
        Debug.Log(gameObject.name + " taking damage " + damage);
        currentHealth -= damage;
        UpdateDamagedMeshes(damage);
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private void CheckForDamage(int health)
    {
        // based on Current health replace mesh or break
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
            GameObject result = Instantiate(environSo.breakableData.objSo.prefab,
                transform.position + Random.insideUnitSphere * 0.5f + dropOffset, Quaternion.identity);
            BaseObj baseObj = result.GetComponent<BaseObj>();
            if (baseObj != null)
            {
                baseObj.Initialize(environSo.breakableData.objSo);
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

    protected virtual void UpdateDamagedMeshes(int damage)
    {
        if (damagedVersions == null || damagedVersions.Count == 0)
            return;

        int index = GetDamageIndex(damage);

        // Clamp index to available damaged meshes
        index = Mathf.Clamp(index, 0, damagedVersions.Count - 1);

        GameObject damagedMesh = damagedVersions[index];
        if (damagedMesh == null)
            return;

        LODGroup lodGroup = GetComponent<LODGroup>();
        if (lodGroup == null)
            return;

        LOD[] lods = lodGroup.GetLODs();

        // Replace renderer in LOD0 (highest detail)
        Renderer newRenderer = damagedMesh.GetComponent<Renderer>();
        if (newRenderer == null)
            return;

        lods[0].renderers = new Renderer[] { newRenderer };

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();
    }

    private int GetDamageIndex(int damage)
    {
        if (damage < damageStage1) return 0;
        if (damage < damageStage2) return 1;
        if (damage < damageStage3) return 2;
        return 3; // fully destroyed (optional)
    }
}

[System.Serializable]
public class PosInEnvironment
{
    public Transform pos;
    public bool isAvailable;
}