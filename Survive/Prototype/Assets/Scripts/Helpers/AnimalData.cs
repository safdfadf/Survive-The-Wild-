using System.Collections.Generic;
using System.Linq;
using Animal.States;
using UnityEngine;

public class AnimalData
{
    public AnimalSo AnimalSo;
    public GameObject AnimalInstance;
    public GameObject AnimalUI;
    public Species Specie;
    private readonly List<Schedule> _dailySchedule;
    public Vector3? CurrentPos; // current pos in the zone 
    public int currentIndex;
    public AnimalHandler AnimalHandler;
    public float NoiseSuspicion;

    public bool IsSpawned;
    public bool isZoneTraveling;

    private MovementSegment _lastSegment;
    private Zone _feedZone;
    private Zone restZone;
    private Zone DrinkingZone;
    private Zone currentZone;
    private Schedule currentSchedule;

    // States for the animal
    public AnimalState currentState { get; private set; }
    private AnimalState calmState;
    private AnimalState alertState;
    private AnimalState alarmState;

    public AnimalData(AnimalSo animalSo, Bounds regionBounds, ScheduleManager scheduleManager,
        AnimalHandler animalHandler)
    {
        AnimalHandler = animalHandler;
        this.AnimalSo = animalSo;
        Specie = animalSo.specie;

        _dailySchedule = scheduleManager.GetSchedule(animalSo.specie, regionBounds);

        foreach (var schedule in _dailySchedule)
        {
            switch (schedule.zoneType)
            {
                case Activity.Feeding:
                    _feedZone = schedule.assignedZone;
                    break;
                case Activity.Resting:
                    restZone = schedule.assignedZone;
                    break;
                case Activity.Drinking:
                    DrinkingZone = schedule.assignedZone;
                    break;
            }
        }

        int hour = TimeManager.Instance.GetCurrentHour();
        currentSchedule = _dailySchedule.FirstOrDefault(s =>
            IsHourInRange(hour, s.startHour, s.endHour));
        // here we need last schedule, 

        if (currentSchedule == null)
        {
            Debug.LogError($"No schedule found for hour {hour} for species {Specie}");
            currentSchedule = _dailySchedule[0];
        }

        switch (currentSchedule.zoneType)
        {
            case Activity.Feeding: currentZone = _feedZone; break;
            case Activity.Resting: currentZone = restZone; break;
            case Activity.Drinking: currentZone = DrinkingZone; break;
        }

        if (currentZone == null)
        {
            Debug.Log(currentZone);
            return;
        }

        CurrentPos = currentZone.RequestPosition() ?? currentZone.transform.position;
        calmState = new CalmState(this);
        alertState = new AlertState(this);
        alarmState = new AlarmState(this);
        currentState = calmState;
    }

    // spawning solly depends on chunk activation however when schedule changes and if current pos lies in active zone spawn it if animal is not already spawned 
    public void OnHourChanged(int hour)
    {
        if (currentState == alarmState) return;
        if (currentSchedule != null && hour == currentSchedule.endHour) // if current state is calm
        {
            Schedule next = GetNextSchedule(currentSchedule);
            BeginTransition(next);
            currentSchedule = next;
            return;
        }
        else
        {
            currentSchedule = GetCurrentScheduleEntry(hour);
            if (currentSchedule == null)
            {
                Debug.Log("current Schedule is null");
                return;
            }

            if (hour == currentSchedule.endHour) // at start current hour would be null 
            {
                BeginTransition(currentSchedule);
            }
        }
    }

    private void BeginTransition(Schedule next)
    {
        _lastSegment = new MovementSegment();
        if (currentZone != null && CurrentPos != null)
        {
            currentZone.ReleasePosition(CurrentPos.Value);
        }

        if (CurrentPos == null) return;
        _lastSegment.StartPos = CurrentPos.Value;
        currentZone = null;

        switch (next.zoneType)
        {
            case Activity.Feeding: currentZone = _feedZone; break;
            case Activity.Drinking: currentZone = DrinkingZone; break;
            case Activity.Resting: currentZone = restZone; break;
        }

        Vector3? pos = currentZone.RequestPosition();
        if (pos == null) return;

        _lastSegment.EndPos = pos.Value;
        CurrentPos = pos;

        if (IsSpawned)
        {
            ScheduledAnimal scheduledAnimal = AnimalInstance.GetComponent<ScheduledAnimal>();
          //  if (!scheduledAnimal.isMoving)
            //    scheduledAnimal.SetIsMoving(false); // ovveride move 
            isZoneTraveling = true;
            scheduledAnimal.MoveTo(CurrentPos.Value, onArrived: () =>
                TrackHandler.instance.CreateTracks(_lastSegment, Specie, currentState, AnimalSo));
            AnimalUI.transform.SetParent(scheduledAnimal.gameObject.transform);
            return;
        }
        else if (ChunkRepo.instance.CheckPosInActiveChunk(CurrentPos.Value))
        {
            Vector3 inActiveChunkPos = GetOutofActiveChunkPos();
            GameObject obj = GlobalPool.instance.Get(AnimalSo.prefab, inActiveChunkPos);
            AnimalInstance = obj;
            ScheduledAnimal scheduledAnimal = obj.GetComponent<ScheduledAnimal>();
            scheduledAnimal.InitializeByData(this);
            scheduledAnimal.MoveTo(CurrentPos
                .Value); // improve further by adding a delay and make them reach at a proper time 
        }

        TrackHandler.instance.CreateTracks(_lastSegment, Specie, currentState, AnimalSo);
        AnimalUI.transform.position = CurrentPos.Value;
    }

    private Schedule GetNextSchedule(Schedule current)
    {
        if (current == null) return null;

        int currentIndex = _dailySchedule.IndexOf(current);
        int nextIndex = (currentIndex + 1) % _dailySchedule.Count;
        return _dailySchedule[nextIndex];
    }

    private void LastSchedule(Schedule last)
    {
        // through the schedule get the zone pos and release and create tracks 
    }

    private Schedule GetCurrentScheduleEntry(int hour)
    {
        return _dailySchedule.FirstOrDefault(s => IsHourInRange(hour, s.startHour, s.endHour));
    }

    private bool IsHourInRange(int hour, int start, int end)
    {
        if (start < end) return hour >= start && hour < end;
        else return hour >= start || hour < end; // overnight case
    }

    public void ChangeState(AnimalState state) // remove states from data 
    {
        currentState = state;
        if (!IsSpawned) return;
        ScheduledAnimal scheduledAnimal = AnimalInstance.GetComponent<ScheduledAnimal>();
        scheduledAnimal.ActivateState(currentState);
    }

    public AnimalState GetCalmState()
    {
        return calmState;
    }

    public AnimalState GetAlertState()
    {
        return alertState;
    }

    public AnimalState GetAlarmState()
    {
        return alarmState;
    }

    public Zone GetCurrentZone()
    {
        return currentZone;
    }

    public Vector3? GetCurrentPosition()
    {
        return CurrentPos;
    }

    public AnimalState GetCurrentState()
    {
        return currentState;
    }

    public Vector3 GetOutofActiveChunkPos()
    {
        Vector3 pos;
        pos = AnimalInstance != null ? AnimalInstance.transform.position : CurrentPos.Value;
        return ChunkRepo.instance.GetPosOutOfActiveChunk(pos);
    }
}