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
    void Update()// instead of updating every frame update it when phone is active 
    {
       if(gameObject.activeInHierarchy)
        currenTime.text = TimeManager.Instance.GetTimeString();
    }
}
