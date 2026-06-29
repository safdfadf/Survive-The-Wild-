using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindSystem : MonoBehaviour // how many script will ask reference for this script? player scent amittoer 
{
    [Header("Wind Settings")]
    private Vector3 windDirection = Vector3.forward;
    private float windSpeed = 2f;

    [Header("Dynamics")]
    public float directionChangeInterval = 30f;
    public float speedMin = 1f;
    public float speedMax = 6f;

    // this script gives direction and speed wind is moving in
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        EventBus.OnRndmTimePassed += UpdateWind;
    }

    private void OnDisable()
    {
        EventBus.OnRndmTimePassed -= UpdateWind;
    }

    private void UpdateWind()// 
    {
        // Random horizontal direction
        Vector2 dir = Random.insideUnitCircle.normalized;
        windDirection = new Vector3(dir.x, 0f, dir.y);

        // Random speed
        windSpeed = Random.Range(speedMin, speedMax);
        EventBus.OnWindChanged?.Invoke(windDirection, windSpeed);
        // call ui manager to update the wind radius 
    }
    public Vector3 GetWindDirection()
    {
        return windDirection;
    }

    public float GetWindSpeed()
    {
        return windSpeed;
    }

}
