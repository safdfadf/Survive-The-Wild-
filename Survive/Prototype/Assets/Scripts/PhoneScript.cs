using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneScript : MonoBehaviour
{
  [SerializeField] private GameObject Compass;
  [SerializeField] private GameObject VitalStats;
   private Animator _animator;
   private bool _isPhoneActive;
   private int _currentIndex;

   private void Awake()
   {
       _animator = GetComponent<Animator>();
       MoveOutPhone();
   }

   public void MoveInPhone()
   {
       _isPhoneActive = true;
       _animator.SetBool("phoneIn",true);
   }

   public void MoveOutPhone()
   {
       _isPhoneActive = false;
       _animator.SetBool("phoneIn",false);
   }
   public void Scroll(InputAction.CallbackContext context)
   {
       if (!_isPhoneActive) return;

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
