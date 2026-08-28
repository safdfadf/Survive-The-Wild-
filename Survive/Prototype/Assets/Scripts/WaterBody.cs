using System;
using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaterBody : MonoBehaviour, IAction, ICollectable // this script will be explored later 
    {
        public bool outlineMe { get; set; }
        public bool canBeCollected { get; set; }
        public GameObject Gm { get; set; }
        public string useMeDescription { get; set; }
        public string Description { get; set; }
        public bool canUse { get; set; }
        public bool canHarvest { get; set; }
        public bool canCraft { get; set; }
        public WaterBodyType bodyType;
        public WaterState waterState;
        [SerializeField] private FoodSo waterSo;
        public GameObject obj { get; set; }

        private void Awake()
        {
            canBeCollected = false;
            outlineMe = false;
            Gm = gameObject;
            useMeDescription = "Drink";
            Description = "Take";
            canHarvest = false;
            canCraft = false;
            canUse = true;
            obj = gameObject;
        }

        private void WashYourself() // it can be used in two ways one wash your self and consume me 
        {
        }

        public void FillContainer()
        {
        }

        public void DrinkMe()
        {
        }

        public void Craft()
        {
        }

        public void Harvest()
        {
            
        }

        public void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(waterSo);
        }
    }

    public enum DirtLevel
    {
        Safe,
        UnSafe,
        Dirty
    }
}