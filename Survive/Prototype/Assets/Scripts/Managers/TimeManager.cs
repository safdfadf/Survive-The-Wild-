using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public int timeScale { get; private set;}= 2;

    private int _startHour = 4;

     private float _currentTimeInMinutes;
     private int _lastHour;


    private int CurrentHour=> Mathf.FloorToInt(_currentTimeInMinutes / 60f)%24;
    public int CurrentMinute => Mathf.FloorToInt(_currentTimeInMinutes) % 60;
    public float CurrentTime => _currentTimeInMinutes;
    // here create a function that invokes an event after every randon duration of time 
    [Header("Random Event Interval (minutes)")]
    [SerializeField] private int minRandomIntervalMinutes = 2;
    [SerializeField] private int maxRandomIntervalMinutes = 10;
    
    private float _nextRandomEventTime;
    private float _lastFiveSecondCheck;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _currentTimeInMinutes = _startHour * 60;
        ScheduleNextRandomEvent();
        
    }

    public int GetCurrentHour()
    {
        return CurrentHour;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTimeInMinutes += Time.deltaTime * timeScale;
        _currentTimeInMinutes %= 1440f; // Wrap around 24 hours

        int currentHour = CurrentHour;
        if (_lastHour != currentHour)
        {
            _lastHour = currentHour;
            EventBus.OnHourChanged?.Invoke(currentHour);
        }
        HandleRandomEvent();
        HandleFiveSecondEvent(); 
    }
    public float GetTimeInMinutes()
    {
        return _currentTimeInMinutes;
    }
    public string GetTimeString()
    {
        return $"{CurrentHour:D2}:{CurrentMinute:D2}";
    }
    private void HandleRandomEvent()
    {
        // Trigger when we "pass" the scheduled time.
        // Must handle wrap-around (e.g., next time is 10 but current is 1435).
        if (HasPassedTime(_currentTimeInMinutes, _nextRandomEventTime))
        {
            EventBus.OnRndmTimePassed?.Invoke(); 
            ScheduleNextRandomEvent();
//            Debug.Log("invoking rndm event");
        }
    }

    private void ScheduleNextRandomEvent()
    {
        int interval = UnityEngine.Random.Range(minRandomIntervalMinutes, maxRandomIntervalMinutes + 1);

        // schedule from NOW
        _nextRandomEventTime = (_currentTimeInMinutes + interval) % 1440f;

       

    }


    private bool HasPassedTime(float current, float target)
    {
      
        float forwardToTarget = (target - current + 1440f) % 1440f;
        // if forward distance is very small, we're at/after target in this frame
        return forwardToTarget <= (Time.deltaTime * timeScale + 0.01f);
    }
    private void HandleFiveSecondEvent()
    {
        // Convert in‑game minutes to seconds
        float currentGameSeconds = _currentTimeInMinutes * 60f;

        if (currentGameSeconds - _lastFiveSecondCheck >= 300f)
        {
            _lastFiveSecondCheck = currentGameSeconds;
            EventBus.On5SecondsPassed?.Invoke();
        }
    }

}
