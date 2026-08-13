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

    [Header("ChooseAttack")] [SerializeField]
    private AnimalAtkBehaviour animalAtkBehaviour;

    [Header("Choose Attack Behavior")] [SerializeField]
    protected AnimalAttack _animalAttack;

    protected NavMeshAgent agent;
    protected Animator animator;
    private int _maxhealth = 100;
    protected int _currentHealth;
    protected int baseDamage = 10;
    protected Species myspecie { get; set; }
    protected HitBox[] hitBoxes;
    public bool isMoving { get; private set; }
    private GameObject _bloodVfx;
    private bool _isAttackComplete;
    [HideInInspector] public AnimalSo AnimalSo;

    //Todo: State manager should handle the states 
    private AnimalStateManager _stateManager;
    protected AnimalState CalmState;
    protected AnimalState AlertState;
    protected AnimalState AlarmState;
    protected AnimalState CurrentState;

    protected Bounds Bounds;

    public bool IsUnscheduled { get; protected set; } = false;

    // a serialized field through which we can decide through the inspector which behavior to choose 
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


    private void OnEnable()
    {
        EventBus.On5SecondsPassed += UpdateCalmState;
    }

    private void LateUpdate()
    {
        if (isMoving)
            animator.SetFloat("Velocity", agent.velocity.magnitude);
        IsPlayerAround();
    }

    private void UpdateCalmState()
    {
        if (CurrentState != CalmState) return;
        CurrentState.UpdateState();
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
            BaseObj baseObj = obj.GetComponent<BaseObj>();
            if (baseObj != null)
            {
                baseObj.Initialize(resourceSo);
                baseObj.rb.isKinematic = false;
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
        Debug.Log("Move");
        if (isMoving)
        {
            Debug.Log("already moving" + gameObject.name);
            return;
        }

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
            RetPosOnNv.TryGetNavMeshPoint(destination, out Vector3 navMeshHit);
            destination = navMeshHit;
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
        MoveTo(PlayerRepository.instance.GetApproachPos().position, () => DoDamage());
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
            isMoving = false;
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }

    private void CheckPlayerPresence() // how should we stop the function fro 
    {
    }

    protected virtual void IsPlayerAround()
    {
    }

    public virtual void DoDamage()
    {
        animator.SetTrigger("attack");
        PlayerRepository.instance.ApplyDamage(_animalAttack);
    }

    public virtual void Attack()
    {
        StartCoroutine(animalAtkBehaviour.Execute(this));
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
//        animator.SetTrigger("Alert");
    }

    protected void LookAtPlayer()
    {
        // 1. Face the player
        Transform player = PlayerRepository.instance.GetPlayerTransform(); // or however you reference player
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f; // prevent tilting
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public IEnumerator RamAttack() 
    {
        float warningRadius = 10f;
        float stopOffset = 1.5f;
        float waitBeforeNextRam = 1f;

        Transform player = PlayerRepository.instance.GetPlayerTransform();
        bool hasAttackedOnce = false;

        while (_currentHealth > _maxhealth * 0.10f)
        {
            bool reached = false;

            while (!reached)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                Vector3 stopPoint = player.position - dirToPlayer * stopOffset;

                if (RetPosOnNv.TryGetNavMeshPoint(stopPoint, out Vector3 navStopPoint))
                    agent.SetDestination(navStopPoint);
                
                if (Vector3.Distance(transform.position, navStopPoint) <= agent.stoppingDistance + 0.2f)
                    reached = true;

                yield return null;
            }

            DoDamage();
            yield return new WaitForSeconds(1f); 
            hasAttackedOnce = true;

            if (hasAttackedOnce)
            {
                float dist = Vector3.Distance(transform.position, player.position);
                if (dist > warningRadius)
                    yield break;
            }

            Vector3 circlePoint = GetRandomPointOnCircle(player.position, warningRadius);

            if (!RetPosOnNv.TryGetNavMeshPoint(circlePoint, out Vector3 navCirclePoint))
            {
                Debug.Log("pos failed");
                Vector3 pos = ChunkManager.Instance.GetClosestInactiveChunkPosition(transform.position);
                MoveTo(pos, () => RemoveAnimal());
                yield break;
            }

            bool arrivedCircle = false;
            MoveTo(navCirclePoint, () => arrivedCircle = true, runSpeed);

            while (!arrivedCircle)
            {
                float dist = Vector3.Distance(transform.position, player.position);
                if (dist > warningRadius)
                {
                    Debug.Log("dis tance greter ");
                    RemoveAnimal();
                    yield break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(waitBeforeNextRam);
            Debug.Log("wait before next");
        }
    }

    protected virtual void RemoveAnimal()
    {
        GlobalPool.instance.Return(AnimalSo.prefab, gameObject);
    }

    private Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        return new Vector3(
            center.x + Mathf.Cos(rad) * radius,
            center.y,
            center.z + Mathf.Sin(rad) * radius
        );
    }
}

[System.Serializable]
public enum AttackType
{
    OneTimeAttack,
    MultiAttack
}

public abstract class AnimalAtkBehaviour : ScriptableObject
{
    public abstract IEnumerator Execute(AnimalBase animal);
}