using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FoodSystem
{
    public class Food : Resource<FoodSo>
    {
        [SerializeField] private Button eatButton;
        private int _health;

        protected override void Awake()
        {
            Gm = gameObject;
            eatButton.onClick.AddListener(EatMe);
            base.Awake();
        }

        private void EatMe()
        {
            PlayerRepository.instance.ConsumeFood(So);
            PlayerRepository.instance.RemoveResourceFromInventory(So, gameObject);
            // remove it from the inventory   
        }
    }
}