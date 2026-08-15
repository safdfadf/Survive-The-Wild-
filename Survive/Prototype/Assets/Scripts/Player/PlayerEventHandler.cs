using UnityEngine;

namespace Player
{
   public class PlayerEventHandler : MonoBehaviour
   {
      private MovementHandler player;
      // use an event here: player controller movement handler currentWeapon.DeliverDamage 
      private void Awake()
      {
         player = GetComponentInParent<MovementHandler>();
      }

      private void DeliverDamage()
      {
         player.CurrentWeapon?.DeliverDamage();
      }
  
   }
}
