using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI currenTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currenTime = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
       if(gameObject.activeInHierarchy)
        currenTime.text = TimeManager.Instance.GetTimeString();
    }
}
