using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WatchUI : MonoBehaviour
{
    [SerializeField] private GameObject Compass;
    [SerializeField] private GameObject VitalStats;
    private Animator _aniator;
    public bool IsPhoneActive { get;  set; }
    private int _currentIndex;


    public void Scroll(InputAction.CallbackContext context)
    {
        if (!IsPhoneActive) return;

        Vector2 scroll = context.ReadValue<Vector2>();

        if (scroll.y > 0)
        {
            _currentIndex++;
        }
        else if (scroll.y < 0)
        {
            _currentIndex--;
        }

        // Clamp between 0 and 1
        _currentIndex = Mathf.Clamp(_currentIndex, 0, 1);

        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        Compass.SetActive(_currentIndex == 0);
        VitalStats.SetActive(_currentIndex == 1);
    }
}