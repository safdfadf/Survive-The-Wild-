using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ScheduledAnimal : AnimalBase
{
    private Activity _currentActivity;
    private Schedule currentSchedule;
    public Zone currentZone { get; private set; }
    public Vector3? currentPos { get; private set; }

    protected AnimalData AnimalData;

    protected override void Awake()
    {
        base.Awake();
    }

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
    private void LateUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState();
        }
    }

    public override void MoveTo(Vector3 destination, Action onArrived = null, float? speedOverride = null)
    {
        base.MoveTo(destination, onArrived, speedOverride);
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