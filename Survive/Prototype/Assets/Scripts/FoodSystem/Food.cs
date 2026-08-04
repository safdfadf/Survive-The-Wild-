using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FoodSystem
{
    public class Food : Resource<FoodSo>
    {
        private int _health;

        protected override void Awake()
        {
            canBeCollected = true;
            Gm = gameObject;
           
            base.Awake();
            
        }

        public override void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(So);
            PlayerRepository.instance.RemoveResourceFromInventory(So, gameObject);
        }

    }
}