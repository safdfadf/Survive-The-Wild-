using System;
using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
using Effect;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaterBody : MonoBehaviour, IInteractable // this script will be explored later 
    {
        public bool OutlineMe { get; set; }
        public bool CanBeCollected { get; set; }
        public GameObject Gm { get; set; }
        public bool isHit { get; set; }
        public Vector3 hitPos { get; set; }

        public bool CanDisplay { get; set; }
        public string UseMeDescription { get; set; }
        public string Description { get; set; }
        public bool CanUse { get; set; }
        public bool CanHarvest { get; set; }
        public bool CanCraft { get; set; }
        public WaterBodyType bodyType;
        public WaterState waterState;
        [SerializeField] private FoodSo waterSo;
        [SerializeField] private GameObject emptyobj;


        private Vector3 lastValidPos;
        private FoodConsumptionData data = new();

        [Header("food poisoning Amount")] [SerializeField]
        private float minPoisonAmount;

        [SerializeField] private float maxPoisonAmount;
        [SerializeField] private EffectsSo parasiteEffect;

        private void Awake()
        {
            CanBeCollected = false;
            OutlineMe = false;
            Gm = gameObject;
            Description = "Drink";
            CanDisplay = true;
            data.NutrientsCount = waterSo.nutrientsCount;
        }

        public void LateUpdate()
        {
            if (!isHit) return;
            emptyobj.transform.position = PlayerRepository.instance.GetPlayerUiPos();
            Gm = emptyobj;// test
        }

        private void WashYourself() // function for use me 
        {
            // remove player's scent 
        }

        public void FillContainer()
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
        }

        public void ExecuteAction()
        {
            CheckForPoisoning();
            PlayerRepository.instance.ConsumeFood(data);
        }

        public FoodConsumptionData GetWater()
        {
            CheckForPoisoning();
            return data;
        }

        private void CheckForPoisoning()
        {
            if (waterState == WaterState.Safe) return;

            float poisonChance = 0f;

            switch (waterState)
            {
                case WaterState.UnSafe:
                    poisonChance = 0.40f; // 40% chance
                    break;

                case WaterState.Dirty:
                    poisonChance = 0.75f; // 75% chance
                    break;
            }

            if (UnityEngine.Random.value <= poisonChance)
            {
                float poisonAmount = UnityEngine.Random.Range(minPoisonAmount, maxPoisonAmount);
                SelfAttack attack = new SelfAttack(Mathf.CeilToInt(poisonAmount), Vector3.zero);
                attack.Effects = parasiteEffect;
                data.SelfAttack = attack;
            }
        }
    }
}