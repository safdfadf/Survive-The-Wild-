using System;
using Player;
using UnityEngine;

public class CompassSystem : MonoBehaviour
{
    private PlayerRepository _playerRepository;

    private void Awake()
    {
        _playerRepository = FindAnyObjectByType<PlayerRepository>();
    }

    private void OnEnable()
    {
        EventBus.OnWindChanged += UpdateArcDirection;
    }

    private void OnDisable()
    {
        EventBus.OnWindChanged += UpdateArcDirection;
    }

    public void Update()// update this every second or even less  
    {
        UpdateCompass();
    }

    private void UpdateCompass()
    {
        Transform playerTransform = _playerRepository.GetPlayerTransform();
        var yaw = playerTransform.eulerAngles.y;
        var quat = Quaternion.Euler(0, 0, yaw);
        UIManager.instance.UpdateCompassRing(quat);
    }

    private void UpdateArcDirection(Vector3 windDirection, float windSpeed)
    {
        float angle = Mathf.Atan2(windDirection.x, windDirection.z) * Mathf.Rad2Deg;
        var Dir = Quaternion.Euler(0, 0, -angle);
        UIManager.instance.UpdateWindDir(Dir);
    }
}