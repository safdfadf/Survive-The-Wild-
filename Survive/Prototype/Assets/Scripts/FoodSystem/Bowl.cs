using System;
using System.Collections.Generic;
using DefaultNamespace.CraftingSystem;
using JetBrains.Annotations;
using Player;
using UnityEngine;

namespace FoodSystem
{
    public class Bowl : Obj<ObjSo>, ICook, ICookable
    {
        [SerializeField] private GameObject bowlInside;

        [SerializeField] private ParticleSystem boilingParticles;

        // this script will also craft new food like stew 
        [SerializeField] private GameObject WaterPrefab;
        [SerializeField] private FoodSo waterSo;
        [SerializeField] [ItemCanBeNull] private List<Transform> waterTransform;

        private List<Ingredient> _ingredients = new();
        private bool _hasWater;
        private Food _foodInBowl;

        protected override void Awake()
        {
            base.Awake();
            useMeDescription = "Drink";
        }

        // how slots will be used : Slots will be used 
        public void StoreObjForCooking(Obj<ObjSo> obj)
        {
            if (_ingredients.Count >= 2)
            {
                //display notification 
                return;
            }

            if (obj.TryGetComponent<Container>(out var container))
            {
                if (!container.IsContainerFilled)
                {
                    // give player notification container is empty
                    return;
                }

                SpawnWaterInBow();
            }
            else if (obj.TryGetComponent<Food>(out var food))
            {
                SubmitFood(food);
            }
        }

        private void SubmitFood(Food food)
        {
            Ingredient foodIng = new();
            foodIng.objSo = food.So;
            foodIng.amount = 1;
            _ingredients.Add(foodIng);
            CheckForRecipeMatch();
        }

        public void ExecuteCooking(float deltaTime)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            UIManager.instance.DisplayCookingSpots(waterTransform, gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            UIManager.instance.ClearAllCookingSpots();
        }


        private void SpawnWaterInBow()
        {
            GameObject water = Instantiate(WaterPrefab, waterTransform[0]);
            Food waterObj = water.GetComponent<Food>();
            waterObj.Initialize(waterSo);
            _hasWater = true;
            Ingredient ing = new();
            ing.objSo = waterSo;
            ing.amount = 1;
            _ingredients.Add(ing);
            canUse = true;
        }

        public override void SetUiBools()
        {
            canCraft = false;
            canHarvest = false;
            canUse = false;
        }

        private void CheckForRecipeMatch()
        {
            var so = PlayerRepository.instance.CheckForRecipeMatch(_ingredients);
            if (so == null) return;
            var obj = GlobalPool.instance.Get(so.resSo.prefab, waterTransform[0].position);
            _foodInBowl = obj.GetComponent<Food>();
            // player needs to cook this 
        }
    }
}