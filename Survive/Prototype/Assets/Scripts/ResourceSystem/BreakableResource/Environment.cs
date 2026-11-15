using System;
using System.Collections.Generic;
using System.Linq;
using SurviveTheWild.Weapon;
using FoodSystem;
using Player;
using SurviveTheWild;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//ToDo: add growth functionality 
public class Environment : MonoBehaviour, ItakeDamage, IsoInitializer<EnvironSo>
{
    [SerializeField] protected int resourceDropCount;
    [SerializeField] protected Vector3 dropOffset;

    public EnvironSo environSo { get; private set; }
    protected int currentHealth;
    protected PosInChunk cashedPosInChunk;
    [Header("Dropables")] [SerializeField] private List<ObjSo> objSos;

    [SerializeField] private List<PosInEnvironment> pos;

    [Header("DamagedVersions")] [SerializeField]
    protected List<GameObject> damagedVersions;

    [Header("Damage Threshold")] [SerializeField]
    private int damageStage1 = 25;

    [SerializeField] private int damageStage2 = 60;
    [SerializeField] private int damageStage3 = 90;

    [Header("Requirement")] [SerializeField]
    private WeaponAbility requiredAbility;

    [Header("Audio")] [SerializeField] private List<AudioClip> damageClips;
    [SerializeField] private AudioClip fallClip;
    public bool IsEnvironment { get; set; }
    public bool IsPlayerInRange { get; set; }
    private FallHandler fallHandler;

    protected virtual void Awake()
    {
        fallHandler = GetComponentInChildren<FallHandler>();
        if (gameObject.name == "TreeR(Clone)" && fallHandler == null)
        {
            print("fall handler is null");
        }
    }

    public void Initialize(EnvironSo so)
    {
        environSo = so;
        IsEnvironment = true;
        SpawnObj();
        if (environSo == null) return;
    }

    private void SpawnObj()
    {
        foreach (var so in objSos)
        {
            for (int i = 0; i < so.amount; i++)
            {
                if (Random.value > so.appearanceProb)
                    continue;

                Vector3 pos = GetPosition();
                if (pos == Vector3.zero) continue;
                GameObject gm = Instantiate(so.prefab, gameObject.transform, true);
                Obj<ObjSo> obj = gm.GetComponent<Obj<ObjSo>>();
                obj.Initialize(so);
                gm.transform.position = pos;
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

    public virtual void TakeDamage(IAttack attack) // some of the environment like trees can only be cut by Ihatch
    {
        PlayerAttack atk = attack as PlayerAttack;
        if (requiredAbility != null && requiredAbility != atk.ability)
        {
            //Show Notification
            UIManager.instance.DisplayNotification("You do not have required Tool");
            return;
        }

        currentHealth -= attack.Damage;
        PlayDamageSound();
        UpdateDamagedMeshes(attack.Damage);
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    private void PlayDamageSound()
    {
        var clip = damageClips[Random.Range(0, damageClips.Count)];
        SceneRoot.Instance.AudioManager.PlaySound(clip);
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
        if (fallHandler != null)
        {
            SceneRoot.Instance.AudioManager.PlaySound(fallClip);
            fallHandler.ExecuteFall(DropObjects, gameObject);
        }
        else
        {
            DropObjects();
        }
    }

    private void DropObjects()
    {
        print("dropping objects");
        for (int i = 0; i < resourceDropCount; i++)
        {
            var result = Instantiate(environSo.breakableData.objSo.prefab,
                transform.position + Random.insideUnitSphere * 0.5f + dropOffset, Quaternion.identity);
            Obj<ObjSo> baseObj = result.GetComponent<Obj<ObjSo>>();
            if (baseObj != null)
            {
                baseObj.Initialize(environSo.breakableData.objSo);
                Rigidbody rb = result.GetComponent<Rigidbody>();
                Collider collider = result.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = true;
                    collider.isTrigger = false;
                }
                else
                {
                    Debug.Log(result.name + " is missing Collider");
                }

                rb.isKinematic = false;
            }

            Destroy(gameObject);
        }
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