using Player;
using UnityEngine;

public class UnScheduleAnimal : AnimalBase // these animals do not have a schedule and spawned around the player
{
    [Header("Detection Setting")] [SerializeField]
    private float alertRadius = 1;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstructionMask;

    [SerializeField] private float alarmRadius = .5f;

    // select Attack Behviour 
    // public animalAttack we choose attack type and its effects ? 
    protected override void Awake()
    {
        IsUnscheduled = true;
        base.Awake();
    }

    public void Initialize(AnimalSo so, Bounds bounds)
    {
        Bounds = bounds;
        AnimalSo = so;
        CreateNewState();
        CalmState.EnterState(this);
    }

    protected override bool IsPlayerAround()
    {
        // here based on distance first alert mode and then attack 
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius, playerMask);

        if (hits.Length == 0)
            return false;

        Transform player = hits[0].transform;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, dir, dist, obstructionMask))
            return false;
        if (dist <= alarmRadius)
        {
            if (CurrentState == null || CurrentState == AlarmState) return false;
            CurrentState = AlarmState;
            CurrentState.EnterState(this);
            Debug.Log("Change to " + CurrentState.ToString());
        }
        else
        {
            if (CurrentState == null || CurrentState == AlertState) return false;
            CurrentState = AlertState;
            CurrentState.EnterState(this);
            Debug.Log("Change to " + CurrentState.ToString());
        }

        return true;
    }

    public override void Attack()
    {
        Debug.Log("Attacking Player");
        animator.SetTrigger("attack");
        LookAtPlayer();
        PlayerRepository.instance.ApplyDamage(_animalAttack);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alarmRadius);
    }
}