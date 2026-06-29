using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
   private MovementHandler player;

   private void Awake()
   {
      player = GetComponentInParent<MovementHandler>();
   }
  
}
