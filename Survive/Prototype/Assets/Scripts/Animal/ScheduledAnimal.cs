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

    protected AnimalData AnimalData;

    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightEye;
    [SerializeField] private float eyeSightDistance = 20f;
    [SerializeField] private float eyeSightAngle = 45f; // half-angle of cone
    [SerializeField] private LayerMask obstructionMask;

    public void InitializeByData(AnimalData animalData)
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
    public override void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null)
    {
        base.MoveTo(destination, onArrived, speedOverride);
    }

    protected override void IsPlayerAround()
    {
        if (leftEye == null || rightEye == null)
            return;

        Vector3 playerPos = PlayerRepository.instance.GetPlayerTransform().position;

        // Eye midpoint
        Vector3 eyeCenter = (leftEye.position + rightEye.position) * 0.5f;

        // Forward direction from eyes
        Vector3 forward = transform.forward;

        // Direction to player
        Vector3 dirToPlayer = (playerPos - eyeCenter).normalized;

        // Check angle
        float angle = Vector3.Angle(forward, dirToPlayer);
        if (angle > eyeSightAngle)
            return;

        // Check distance
        float dist = Vector3.Distance(eyeCenter, playerPos);
        if (dist > eyeSightDistance)
            return ;

        // Check line of sight
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
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public GameObject GetFollowPoint()
    {
        return followPoint;
    }
}