using Player;
using UnityEngine;

public class UnScheduleAnimal : AnimalBase // these animals do not have a schedule and spawned around the player
{
    [Header("Detection Setting")] [SerializeField]
    private float alertRadius = 1;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstructionMask;

    [SerializeField] private float alarmRadius = .8f;

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

    protected override void IsPlayerAround()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius, playerMask);

        if (hits.Length == 0)
            return;

        Transform player = hits[0].transform;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, dir, dist, obstructionMask))
        {
            Debug.Log(" i am returning");
            return;
        }

        if (dist <= alarmRadius)
        {
            Debug.Log("Player too close");

            if (CurrentState != AlarmState)
            {
                CurrentState = AlarmState;
                CurrentState.EnterState(this);
                Debug.Log("Switched to AlarmState");
            }

            return;
        }

        // ALERT ZONE
        if (CurrentState != AlertState)
        {
            CurrentState = AlertState;
            CurrentState.EnterState(this);
            Debug.Log("Switched to AlertState");
        }

        return;
    }

    public override void DoDamage() 
    {
        LookAtPlayer();
        base.DoDamage();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alarmRadius);
    }
}