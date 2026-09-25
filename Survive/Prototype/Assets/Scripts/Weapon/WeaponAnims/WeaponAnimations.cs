using System;
using UnityEngine;

namespace DefaultNamespace.Weapon.WeaponAnims
{
    public class WeaponAnimations : MonoBehaviour
    {
       protected PlayerAnimator animator;

       private void Awake()
       {
           animator = FindAnyObjectByType<PlayerAnimator>();
       }
    }
}