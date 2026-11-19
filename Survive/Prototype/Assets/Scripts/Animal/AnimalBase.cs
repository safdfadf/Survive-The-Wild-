using System;
using System.Collections;
using Animal.States;
using SurviveTheWild.EventBus;
using SurviveTheWild.Interface;
using SurviveTheWild.Weapon;
using Player;
using SurviveTheWild.Animal.AtkBehaviors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AnimalBase : MonoBehaviour, IInteractable
{
    [Header("Movement Info")] [SerializeField]
    protected float walkSpeed;

    [SerializeField] protected float alertSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected float fleeSpeed;
    [SerializeField] public GameObject followPoint;

    [FormerlySerializedAs("_attackBehaviour")] [Header("ChooseAttack")] [SerializeField]
    private Attacks atkPrefab;

    private Attacks _atk;

    [Header("Requirement")] [SerializeField]
    private WeaponAbility requiredAbility;

    public NavMeshAgent agent;
    protected Animator animator;
    private int _maxhealth = 100;
    protected int _currentHealth;

    public int CurrentHealth => _currentHealth;
    protected int baseDamage = 10;
    protected Species myspecie { get; set; }
    protected HitBox[] hitBoxes;
    public bool isMoving { get; private set; }
    private GameObject _bloodVfx;
    private bool _isAttackComplete;

    [Header("Death Choice")] [SerializeField]
    private bool _instantDeath;

    [HideInInspector] public AnimalSo AnimalSo;


    //Todo: State manager should handle the states 
    private AnimalStateManager _stateManager;
    protected AnimalState CalmState;
    protected AnimalState AlertState;
    protected AnimalState AlarmState;
    protected AnimalState CurrentState;
    [SerializeField] private PuddleFx _puddleFx;
    protected Bounds Bonds;

    public bool CanDisplay { get; set; }
    public string UseMeDescription { get; set; }
    public string Description { get; set; }
    public bool CanUse { get; set; }
    public bool CanHarvest { get; set; }
    public bool CanCraft { get; set; }

    public GameObject obj { get; set; }
    public bool IsUnscheduled { get; protected set; } = false;

    public bool OutlineMe { get; set; }
    public bool CanBeCollected { get; set; }
    public GameObject Gm { get; set; }
    public bool isHit { get; set; }
    public Vector3 hitPos { get; set; }


    public bool IsAggresive { get; set; }
    private bool drawCircle;
    private bool _canUpdateCalmState = true;
    private float _currentSpeed;

    protected virtual void Awake()
    {
        _currentHealth = _maxhealth;
        agent = GetComponentInChildren<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (VfxProvider.Instance != null)
            _bloodVfx = VfxProvider.Instance.GetBloodParticle();
        hitBoxes = GetComponentsInChildren<HitBox>();
        _puddleFx = GetComponentInChildren<PuddleFx>();
        foreach (HitBox h in hitBoxes)
        {
            h.Initialize(this);
        }

        CanDisplay = false;

        CanUse = false;
        CanHarvest = true;
        CanCraft = false;
        CanBeCollected = false;
        OutlineMe = false;
        Gm = gameObject;
        obj = gameObject;
        Description = "Skin";
    }

    private void Start()
    {
        ToggleCollider(false);
    }

    private void OnEnable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += UpdateCalmState;
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += IsPlayerAround;
        SceneRoot.Instance.EventManager.AnimalEvents.onUpdateLodLevel += UpdateLod;
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= UpdateCalmState;
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= IsPlayerAround;
        SceneRoot.Instance.EventManager.AnimalEvents.onUpdateLodLevel -= UpdateLod;
    }

    private void LateUpdate()
    {
        if (isMoving)
            animator.SetFloat("Velocity", agent.velocity.magnitude/_currentSpeed);
    }

    private void UpdateLod(LodLevel Level)
    {
        switch (Level)
        {
            case LodLevel.Lod0:
                SetToLod0();
                break;
            case LodLevel.Lod1:
                SetToLod1();
                break;
            case LodLevel.Lod2:
                SetToLod2();
                break;
        }
    }

    private void SetToLod0()
    {
        print("enabling lod0" + gameObject.name);
        animator.enabled = true;
        _canUpdateCalmState = true;
    }

    private void SetToLod1()
    {
        print("enabling lod1" + gameObject.name);
        animator.enabled = true;
        _canUpdateCalmState = true;
    }

    private void SetToLod2()
    {
        animator.enabled = false;
        _canUpdateCalmState = false;
    }

    private void UpdateCalmState()
    {
        if (CurrentState == null || !_canUpdateCalmState) return;
        if (AnimalSo == null)
        {
            print("can't update calm state Animal So");
            return;
        }

        CurrentState.UpdateState();
    }

    public virtual void TakeDamage(PlayerAttack atk)
    {
        if (_currentHealth <= 0) return;
        _currentHealth -= atk.Damage;
        print(_currentHealth);
        if (_currentHealth <= 0)
        {
            DecideWayOfDeath(atk.HitLocation);
        }
    }

    public bool IsLowHealth()
    {
        return _currentHealth <= 20; // prec = part/100 * 100
    }

    private void DecideWayOfDeath(DamageLocation location)
    {
        if (location == DamageLocation.Head || _instantDeath)
        {
            InstantDeath();
        }
        else if (location == DamageLocation.MiddleBody)
        {
            if (Random.value < 0.5f)
            {
                RunDie();
            }
            else
            {
                InstantDeath();
            }
        }
        else
        {
            RunDie();
        }
    }

    private void PlayBloodVfx()
    {
        StartCoroutine(_puddleFx.ActivateFx());
    }

    public void InstantDeath()
    {
        SceneRoot.Instance.EventManager.AnimalEvents.AnimalDeath(this);
        ToggleCollider(true);
        animator.SetBool("Death", true);
        agent.enabled = false;
        agent.speed = 0;
        CanDisplay = true;
        _canUpdateCalmState = false;
        PlayBloodVfx();
        if (_atk != null) Destroy(_atk.gameObject);
    }

    private void RunDie()
    {
        print("run die ");
        var presentChunk = ChunkManager.Instance.GetChunkAtPos(transform.position);
        var destination = RetPosOnNv.ReturnRandomNavMeshPos(presentChunk.bounds);
        MoveTo(destination, InstantDeath);
    }

    private void DropResource()
    {
        ObjSo objSo = AnimalSo.collectable;
        GameObject obj = GlobalPool.instance.Get(objSo.prefab, transform.position + new Vector3(0, .1f, 0));
        Obj<ObjSo> baseObj = obj.GetComponent<Obj<ObjSo>>();
        if (baseObj != null)
        {
            baseObj.Initialize(objSo);
            baseObj.rb.isKinematic = false;
        }

        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }

        GlobalPool.instance.Return(AnimalSo.prefab, gameObject);
        SceneRoot.Instance.EventManager.inventoryEvents.GatherResource(baseObj.gameObject); //Todo:  remove this 
    }

    public void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null,
        bool ovveride = false)
    {
        if (isMoving && !ovveride)
        {
            return;
        }

        agent.stoppingDistance = .1f;
        agent.speed = speedOverride ?? runSpeed;

        _currentSpeed = agent.speed;

        if (!IsValidNavMeshPosition(destination))
        {
            RetPosOnNv.TryGetNavMeshPoint(destination, out Vector3 navMeshHit);
            destination = navMeshHit;
        }

        agent.SetDestination(destination);
        isMoving = true;
        animator.SetFloat("Velocity", agent.desiredVelocity.magnitude);
        StartCoroutine(MonitorArrival(onArrived));
    }

    bool IsValidNavMeshPosition(Vector3 pos, float radius = 1f)
    {
        return NavMesh.SamplePosition(pos, out _, radius, NavMesh.AllAreas);
    }

    protected virtual IEnumerator MonitorArrival(Action onArrived)
    {
        if (agent.isOnNavMesh)
        {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            onArrived?.Invoke();
            isMoving = false;
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }


    protected virtual void IsPlayerAround()
    {
    }

    public virtual void DoDamage(AnimalAttack attack)
    {
        animator.SetTrigger("attack");
        PlayerRepository.instance.ApplyDamage(attack);
    }

    public virtual void Attack()
    {
        InitializeAttack();
        _atk.Execute();
    }

    private void InitializeAttack()
    {
        var obj = GlobalPool.instance.Get(atkPrefab.gameObject, Vector3.zero);
        _atk = obj.GetComponent<Attacks>();
        _atk.Initialize(this);
    }

    public void CreateNewState()
    {
        // create new state 
        CalmState = new CalmState();
        AlertState = new AlertState();
        AlarmState = new AlarmState();
        CurrentState = CalmState;
    }

    public void TriggerAlertAnim()
    {
        // trigger alarm audio 
    }


    public void GetOutOfChunk()
    {
        print("get Out of Chunk");

        var pos = ChunkManager.Instance.GetClosestInactiveChunkPosition(transform.position);
        RetPosOnNv.TryGetNavMeshPoint(pos, out Vector3 navMeshHit);
        MoveTo(navMeshHit, () => RemoveAnimal(), null, true);
    }

    protected virtual void RemoveAnimal()
    {
        print("remove animal");
        if (_atk != null) Destroy(_atk.gameObject);
        GlobalPool.instance.Return(AnimalSo.prefab, gameObject);
    }

    public void Craft()
    {
    }

    public void Harvest()
    {
    }

    public virtual void ExecuteAction()
    {
        BaseWeapon weapon = PlayerRepository.instance.GetCurrentWeapon();

        if (weapon == null || weapon.Ability == null || weapon.Ability != requiredAbility)
        {
            print(weapon + "" + "weapon.Ability");
            UIManager.instance.DisplayNotification("You dont have the required tool");
            return;
        }

        StartCoroutine(Skinning());
    }

    private IEnumerator Skinning()
    {
        // skinning effetc 
        yield return new WaitForSeconds(.5f);
        DropResource();
    }

    public void UseMe()
    {
    }

    private void ToggleCollider(bool toggle)
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = toggle;
        foreach (var box in hitBoxes)
        {
            box.ToggleCollider(!toggle);
        }
    }

    public virtual void HerdCall()
    {
    }

    public virtual void CamlBehavior()
    {
        Chunk chunk = ChunkManager.Instance.GetChunkAtPos(transform.position);
        Vector3 pos = RetPosOnNv.ReturnRandomNavMeshPos(chunk.bounds);
        MoveTo(pos);
    }

    public virtual void AlertBehavior()
    {
        animator.SetTrigger("Alert");
    }

    protected virtual void AlarmBehavior()
    {
    }
}


public abstract class AnimalAtkBehaviourSo : ScriptableObject
{
    [SerializeField] public Attacks attacks;
    public abstract IEnumerator Execute(AnimalBase animal);
}