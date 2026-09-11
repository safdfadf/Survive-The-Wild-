using System;
using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
using Effect;
using FoodSystem;
using Player;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaterBody : MonoBehaviour, IInteractionUI, IInteractable // this script will be explored later 
    {
        public bool outlineMe { get; set; }
        public bool canBeCollected { get; set; }
        public GameObject Gm { get; set; }
        public bool isHit { get; set; }
        public Vector3 hitPos { get; set; }
        public bool canDisplay { get; set; }
        public string useMeDescription { get; set; }
        public string Description { get; set; }
        public bool canUse { get; set; }
        public bool canHarvest { get; set; }
        public bool canCraft { get; set; }
        public WaterBodyType bodyType;
        public WaterState waterState;
        [SerializeField] private FoodSo waterSo;
        [SerializeField] private GameObject emptyobj;
        public GameObject obj { get; set; }

        private Vector3 lastValidPos;
        private FoodConsumptionData data = new();

        [Header("food poisoning Amount")] [SerializeField]
        private float minPoisonAmount;

        [SerializeField] private float maxPoisonAmount;
        [SerializeField] private EffectsSo parasiteEffect;

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
            canDisplay = true;
            obj = emptyobj;
            data.NutrientsCount = waterSo.nutrientsCount;
        }

        public void LateUpdate()
        {
            if (!isHit) return;
            emptyobj.transform.position = PlayerRepository.instance.GetPlayerUiPos();
        }

        private void WashYourself() // it can be used in two ways one wash your self and consume me 
        {
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
            CheckForPoisoning();
            PlayerRepository.instance.ConsumeFood(data);
        }

        private void CheckForPoisoning()
        {
            // if current water state is unsfae
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