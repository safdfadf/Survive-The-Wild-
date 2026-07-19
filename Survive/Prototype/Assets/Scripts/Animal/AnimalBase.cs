using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBase : MonoBehaviour
{
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float alertSpeed;
    [SerializeField] protected float runSpeed;
    [SerializeField] protected float fleeSpeed;
    [SerializeField] protected GameObject followPoint;
    protected NavMeshAgent agent;
    protected Animator animator;
    private Activity _currentActivity;
    private int _maxhealth = 100;
    private int _currentHealth;
    private int baseDamage = 10;
    protected Species myspecie { get; set; }

    private Schedule currentSchedule;
    public Zone currentZone { get; private set; }
    public Vector3? currentPos { get; private set; }

    protected AnimalData AnimalData;
    protected AnimalSo AnimalSo;

    private AnimalStateManager _stateManager;
    protected AnimalState CalmState;
    protected AnimalState AlertState;
    protected AnimalState AlarmState;
    protected AnimalState CurrentState;
    protected HitBox[] hitBoxes;
    public bool isMoving { get; private set; }
    private GameObject _bloodVfx;


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

    public void Initialize(AnimalData animalData)
    {
        AnimalData = animalData;
        AnimalSo = animalData.AnimalSo;
        CurrentState = animalData.GetCurrentState();
        CalmState = animalData.GetCalmState();
        AlertState = animalData.GetAlertState();
        AlarmState = animalData.GetAlarmState();
        currentZone = animalData.GetCurrentZone();
        currentPos = animalData.GetCurrentPosition();
        ActivateState(CurrentState);
    }

    protected virtual void Update()
    {
        animator.SetFloat("Velocity", agent.velocity.magnitude);
    }

    private void LateUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState();
        }
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

    private void Death() // use generic spawner to spawn  
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

    private IEnumerator ActivateRagdoll()
    {
        yield return new WaitForSeconds(1f);
        RagdollToggle Rt = GetComponent<RagdollToggle>();
        Rt.RagdollActive(true);
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
        StartCoroutine(MonitorArrival(onArrived));
    }

    bool IsValidNavMeshPosition(Vector3 pos, float radius = 1f)
    {
        return NavMesh.SamplePosition(pos, out _, radius, NavMesh.AllAreas);
    }

    private IEnumerator MonitorArrival(Action onArrived)
    {
        if (agent.isOnNavMesh)
        {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            onArrived?.Invoke();
            isMoving = false;
            AnimalData.isZoneTraveling = false;
            Debug.Log("Reached the destination");
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }

    public void AnimalWrap(Vector3 position)
    {
        agent.Warp(position);
    }

    public void ActivateState(AnimalState newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public void SetIsMoving(bool value)
    {
        isMoving = value;
    }

    public GameObject GetFollowPoint()
    {
        return followPoint;
    }

    public void ToglleEatAnim(bool toggle)
    {
        animator.SetBool("Eat", toggle);
    }
}