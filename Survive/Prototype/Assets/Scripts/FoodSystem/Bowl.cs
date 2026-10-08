using System;
using System.Collections.Generic;
using DefaultNamespace.CraftingSystem;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace FoodSystem
{
    public class Bowl : Obj<ObjSo>, ICook, ICookable
    {
        [SerializeField] private FoodSo waterSo;
        private List<Transform> waterTransform = new();

        private List<Ingredient> _ingredients = new();
        private Food _foodInBowl;
        
        protected override void Awake()
        {
            base.Awake();
            UseMeDescription = "Drink";
            waterTransform.Add(transform);
        }

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

                SpawnWaterInBow(container);
            }
            else if (obj.TryGetComponent<Food>(out var food))
            {
                SubmitFood(food);
            }
        }

        private void SubmitFood(Food food)
        {
            Ingredient foodIng = new()
            {
                objSo = food.So,
                amount = 1
            };
            food.gameObject.transform.position = waterTransform[0].position;
            _ingredients.Add(foodIng);
            CheckForRecipeMatch();
        }

        public void ExecuteCooking(float deltaTime)
        {
            if (_foodInBowl == null) return;
            _foodInBowl.ExecuteCooking(deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            UIManager.instance.DisplayCookingSpots(waterTransform, gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            UIManager.instance.ClearAllCookingSpots();
        }


        private void SpawnWaterInBow(Container container)
        {
            GameObject water = Instantiate(waterSo.prefab, waterTransform[0]);
            Food waterObj = water.GetComponent<Food>();
            waterObj.Initialize(waterSo);
            Ingredient ing = new()
            {
                objSo = waterSo,
                amount = 1
            };
            _ingredients.Add(ing);
            CanUse = true;
            _foodInBowl = waterObj;
            _foodInBowl.data = container.foodConsumptionData;
        }
        private void CheckForRecipeMatch()
        {
            var so = PlayerRepository.instance.CheckForRecipeMatch(_ingredients);
            if (so == null) return;


            var obj = GlobalPool.instance.Get(so.resSo.prefab, waterTransform[0].position);
            _foodInBowl = obj.GetComponent<Food>();
        }
    }
}