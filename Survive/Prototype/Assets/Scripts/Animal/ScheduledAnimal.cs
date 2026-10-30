using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class ScheduledAnimal : AnimalBase
{
    private Activity _currentActivity;
    private Schedule currentSchedule;
    public Zone currentZone { get; private set; }
    public Vector3? currentPos { get; private set; }

    public AnimalData AnimalData { get; private set; }


    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private float eyeSightDistance = 20f;
    [SerializeField] private float eyeSightAngle = 45f; // half-angle of cone
    [SerializeField] private LayerMask obstructionMask;

    public void InitializeByData(AnimalData animalData)
    {
        AnimalData = animalData;
        AnimalSo = animalData.AnimalSo;
        IsAggresive = AnimalSo.isAggresive;
        // CurrentState = animalData.GetCurrentState();
        CalmState = animalData.GetCalmState();
        AlertState = animalData.GetAlertState();
        AlarmState = animalData.GetAlarmState();
        currentZone = animalData.GetCurrentZone();
        currentPos = animalData.GetCurrentPosition();
        ActivateState( animalData.GetCurrentState());
    }

    protected override void IsPlayerAround()
    {
        if (leftEye == null || rightEye == null)
            return;

        Vector3 playerPos = PlayerRepository.instance.GetPlayerTransform().position;

        Vector3 eyeCenter = (leftEye.position + rightEye.position) * 0.5f;

        Vector3 forward = transform.forward;

        Vector3 dirToPlayer = (playerPos - eyeCenter).normalized;

        float angle = Vector3.Angle(forward, dirToPlayer);
        if (angle > eyeSightAngle)
            return;

        float dist = Vector3.Distance(eyeCenter, playerPos);
        if (dist > eyeSightDistance)
            return;

        if (Physics.Raycast(eyeCenter, dirToPlayer, dist, obstructionMask))
            return;

        return;
    }

    public void AnimalWrap(Vector3 position)
    {
        agent.Warp(position);
    }


    public void ActivateState(AnimalState newState)
    {
        if (CurrentState != null && CurrentState == newState) return;
        if (CurrentState != null)
            CurrentState.ExitState(); 
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public GameObject GetFollowPoint()
    {
        return followPoint;
    }

    public override void TakeDamage(PlayerAttack atk)
    {
        base.TakeDamage(atk);
        AnimalData.isLeader = true; // whoever is hurt in herd should call the leader instead of it becoming
        // call audio and an hurt anim and then run 
        ActivateState(AlarmState);
    }

    protected override void RemoveAnimal()
    {
        Vector3 pos = ChunkManager.Instance.GetClosestInactiveChunkPosition(transform.position);
        MoveTo(pos, () => AnimalData.AnimalHandler.DeactivateAnimal(AnimalData)
        );
    }

    public override void HerdCall()
    {
        if (AnimalData.AnimalHandler.Attacker == null && IsAggresive)
        {
            AnimalData.AnimalHandler.Attacker = this;
        }

        AnimalData.AnimalHandler.HerdWarning(this, AnimalData.GetCurrentZone());
    }
}