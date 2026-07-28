using System;
using System.Collections;
using Animal.States;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBase : MonoBehaviour
{
    [Header("Movement Info")] [SerializeField]
    protected float walkSpeed;

    [SerializeField] protected float alertSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected float fleeSpeed;
    [SerializeField] public GameObject followPoint;

    [Header("ChooseAttackType")] [SerializeField]
    protected bool isPoison;

    [SerializeField] protected bool isFire;
    [SerializeField] protected bool isStun;

    [Header("Choose Attack Behavior")] [SerializeField]
    private AttackType attackType;

    protected NavMeshAgent agent;
    protected Animator animator;
    private int _maxhealth = 100;
    protected int _currentHealth;
    protected int baseDamage = 10;
    protected Species myspecie { get; set; }
    protected HitBox[] hitBoxes;
    public bool isMoving { get; private set; }
    private GameObject _bloodVfx;

    [HideInInspector] public AnimalSo AnimalSo;

    //Todo: State manager should handle the states 
    private AnimalStateManager _stateManager;
    protected AnimalState CalmState;
    protected AnimalState AlertState;
    protected AnimalState AlarmState;
    protected AnimalState CurrentState;

    protected Bounds Bounds;
    public bool IsUnscheduled { get; protected set; } = false;

    protected virtual void Awake()
    {
        _currentHealth = _maxhealth;
        agent = GetComponentInChildren<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (VfxProvider.Instance != null)
            _bloodVfx = VfxProvider.Instance.GetBloodParticle();
        hitBoxes = GetComponentsInChildren<HitBox>();
        foreach (HitBox h in hitBoxes)
        {
            h.Initialize(this);
        }
    }

    protected virtual void Update()
    {
       
    }

    private void LateUpdate()
    {
        if (isMoving)
            animator.SetFloat("Velocity", agent.velocity.magnitude);
        IsPlayerAround();
        if(CurrentState == null)return;
    }

    public void TakeDamage(int damage, Vector3 contact)
    {
        if (_currentHealth <= 0) return;
        PlayBloodVfx(contact);
        int totalDamage = baseDamage * damage; // health = 100, 10 * 8
        _currentHealth -= totalDamage;
        Debug.Log(myspecie + "remaing Health" + _currentHealth);
        if (_currentHealth <= 0)
        {
            Death();
        }
        else
        {
            // change to alert state 
        }
    }

    private void PlayBloodVfx(Vector3 contact)
    {
        GameObject obj = GlobalPool.instance.Get(_bloodVfx, contact);
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        ps.Play();
        StartCoroutine(ReturnAfter(1f, obj));
    }

    private IEnumerator ReturnAfter(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        GlobalPool.instance.Return(_bloodVfx, obj);
    }

    protected virtual void Death()
    {
        animator.SetBool("Death", true);
        agent.enabled = false;
        agent.speed = 0;
        if (AnimalSo != null)
        {
            ResourceSo resourceSo = AnimalSo.resourceSo;
            GameObject obj = Instantiate(resourceSo.prefab, transform.position + new Vector3(0, .5f, 0),
                Quaternion.identity);
            BaseResource baseResource = obj.GetComponent<BaseResource>();
            if (baseResource != null)
            {
                baseResource.Initialize(resourceSo);
                baseResource.SetKinematic(false);
            }

            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
                collider.isTrigger = true;
            }
        }
    }

    public virtual void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null)
    {
        if (isMoving) return;
        agent.stoppingDistance = .1f;
        if (agent == null)
        {
            Debug.Log("agent is null");
            return;
        }

        if (speedOverride > 14 || speedOverride == null)
        {
            speedOverride = runSpeed;
            agent.speed = speedOverride.Value;
        }

        if (!IsValidNavMeshPosition(destination))
        {
            Debug.Log("Destination is invalid");
        }

        agent.SetDestination(destination);
        isMoving = true;
        animator.SetFloat("Velocity", agent.desiredVelocity.magnitude);
        StartCoroutine(MonitorArrival(onArrived));
    }

    public void MoveToPlayerForAttack()
    {
        Debug.Log("Moving To Player");
        // keep running after the player until you attack 
        MoveTo(PlayerRepository.instance.GetApproachPos().position, () => Attack());
    }

    public virtual void MoveInBounds() // this function can be shared by all the animals to move in zone/chunk 
    {
        if (Bounds.size == Vector3.zero) return;
        Vector3 randomPoint = RetPosOnNv.ReturnRandomNavMeshPos(Bounds);
        MoveTo(randomPoint);
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
            Debug.Log("Arrived");
            isMoving = false;
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }

    private void CheckPlayerPresence() // how should we stop the function fro 
    {
      
       
    }

    protected virtual bool IsPlayerAround()
    {
        return false;
    }

    public virtual void Attack()
    {
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
        LookAtPlayer();   
        animator.SetTrigger("Alert");
    }
    protected void LookAtPlayer()
    {
        // 1. Face the player
        Transform player = PlayerRepository.instance.GetPlayerTransform(); // or however you reference player
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f; // prevent tilting
        transform.rotation = Quaternion.LookRotation(dir);
    }
}

[System.Serializable]
public enum AttackType
{
    OneTimeAttack,
    MultiAttack
}