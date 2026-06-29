using System;
using UnityEngine;

public class SunController : MonoBehaviour
{
    private void Update()
    {
        float timePrecent = TimeManager.Instance.CurrentTime / 1440f;
        transform.rotation = Quaternion.Euler(new Vector3((timePrecent * 360f)-90f, 0f, 0f));
    }
}
