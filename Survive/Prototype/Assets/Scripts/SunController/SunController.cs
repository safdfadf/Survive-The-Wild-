using System;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private void OnEnable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += UpdateSun;
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= UpdateSun;
    }

    private void UpdateSun()
    {
        var timePrecent = TimeManager.Instance.CurrentTime / 1440f;
        transform.rotation = Quaternion.Euler(new Vector3((timePrecent * 360f) - 90f, 0f, 0f));
    }
}