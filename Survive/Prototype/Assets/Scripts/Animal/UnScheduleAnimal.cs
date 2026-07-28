using Player;
using UnityEngine;

public class UnScheduleAnimal : AnimalBase // these animals do not have a schedule and spawned around the player
{
    [SerializeField] private float alertRadius = 10f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstructionMask;

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
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius, playerMask);

        if (hits.Length == 0)
            return false;

        Transform player = hits[0].transform;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, dir, dist, obstructionMask))
            return false;

        return true;
    }

    
    protected override void Attack()
    {
        AnimalAttack attack = new AnimalAttack(AnimalSo.damage, isPoison, isStun, isFire);
        PlayerRepository.instance.ApplyDamage(attack);

    }
}