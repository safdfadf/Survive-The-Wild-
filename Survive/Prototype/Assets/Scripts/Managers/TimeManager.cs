using System;
using System.Collections;
using SurviveTheWild.EventBus;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    [Header("Time Scale")] [SerializeField]
    public float timeScale = 2f;

    [Header("Starting Date")] [SerializeField]
    private int startYear = 1;

    [SerializeField] private int startMonth = 1;
    [SerializeField] private int startDay = 1;
    [SerializeField] private int startHour = 4;
    [SerializeField] private int startMinute = 0;

    [Header("Calendar")] [SerializeField] private int daysPerMonth = 30;
    [SerializeField] private int monthsPerYear = 12;

    [Header("Random Event Interval (Minutes)")] [SerializeField]
    private int minRandomIntervalMinutes = 2;

    [SerializeField] private int maxRandomIntervalMinutes = 10;

    private float totalWorldMinutes;
    private float nextRandomEventTime;

    private int lastHour = -1;
    private int lastDay = -1;
    private int lastMonth = -1;
    private int lastYear = -1;

    #region Time Properties

    public int CurrentMinute =>
        Mathf.FloorToInt(totalWorldMinutes) % 60;

    public int CurrentHour =>
        Mathf.FloorToInt(totalWorldMinutes / 60f) % 24;

    public int CurrentDay =>
        (Mathf.FloorToInt(totalWorldMinutes / (60f * 24f)) %
         daysPerMonth) + 1;

    public int CurrentMonth =>
        (Mathf.FloorToInt(totalWorldMinutes /
                          (60f * 24f * daysPerMonth))
         % monthsPerYear) + 1;

    public int CurrentYear =>
        Mathf.FloorToInt(totalWorldMinutes /
                         (60f * 24f * daysPerMonth * monthsPerYear))
        + 1;

    #endregion

    public int TotalDaysPassed =>
        (CurrentYear - 1) * monthsPerYear * daysPerMonth +
        (CurrentMonth - 1) * daysPerMonth +
        CurrentDay;

    private void Awake()
    {
        totalWorldMinutes =
            ((startYear - 1) * monthsPerYear * daysPerMonth * 24 * 60) +
            ((startMonth - 1) * daysPerMonth * 24 * 60) +
            ((startDay - 1) * 24 * 60) +
            (startHour * 60) +
            startMinute;

        lastHour = CurrentHour;
        lastDay = CurrentDay;
        lastMonth = CurrentMonth;
        lastYear = CurrentYear;

        ScheduleNextRandomEvent();
    }

    private void Start()
    {
        StartCoroutine(OneSecondCheck());
    }

    private void Update()
    {
        totalWorldMinutes += Time.deltaTime * timeScale;

        HandleHourChanged();
        HandleRandomEvent();
    }

    private void HandleHourChanged()
    {
        if (CurrentHour != lastHour)
        {
            lastHour = CurrentHour;

            SceneRoot.Instance.EventManager
                .TimeEvents
                .onOneHourPass(CurrentHour);
        }

        if (CurrentDay != lastDay)
        {
            lastDay = CurrentDay;

            SceneRoot.Instance.EventManager
                .TimeEvents
                .onDayPassed?.Invoke(CurrentDay);
        }
    }

    private void HandleRandomEvent()
    {
        if (totalWorldMinutes >= nextRandomEventTime)
        {
            EventBus.OnRndmTimePassed?.Invoke();

            ScheduleNextRandomEvent();
        }
    }

    private void ScheduleNextRandomEvent()
    {
        int interval = Random.Range(
            minRandomIntervalMinutes,
            maxRandomIntervalMinutes + 1);

        nextRandomEventTime =
            totalWorldMinutes + interval;
    }

    private IEnumerator OneSecondCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            SceneRoot.Instance
                .EventManager
                .TimeEvents
                .oneSecondPass();
        }
    }

    public string GetTimeString()
    {
        return $"{CurrentHour:D2}:{CurrentMinute:D2}";
    }

    public string GetDateString()
    {
        return $"Day {CurrentDay} Month {CurrentMonth} Year {CurrentYear}";
    }

    public float GetTotalWorldMinutes()
    {
        return totalWorldMinutes;
    }
}