using UnityEngine;
using UnityEngine.UI;

public class Bandage : BaseResource,IHeal// this is a resourse and an imventory intem 
{
   [SerializeField] private Button useMeButton;
   protected override void Awake()
   {
      useMeButton.onClick.AddListener(HealPlayer);
      base.Awake();
   }

   private void UseMe()
   {
      // Use Bandage
   }

   public void HealPlayer()
   {
      
   }
}
