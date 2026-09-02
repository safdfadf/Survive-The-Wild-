using System;
using DefaultNamespace.Interface;
using DefaultNamespace.ResourceSystem;
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
        [SerializeField] private float allowedRadius = 2f;
        private Vector3 lastValidPos;

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
        }

        public void LateUpdate()
        {
            if (!isHit) return;
            emptyobj.transform.position = new Vector3(hitPos.x, 0, hitPos.z);
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

        private bool IsOutOfBounds(Vector3 position) //1 we need to make sure player is in water bounderies 
            //2) when player is water bounds only show it in alllowed radius which is updated based on player pos 
        {
            float dist = Vector3.Distance(transform.position, position);
            return dist > allowedRadius;
        }

        public void Harvest()
        {
        }

        public void UseMe()
        {
            PlayerRepository.instance.ConsumeFood(waterSo);
        }
        // first we need to know ray cast is hitting thi object 
        // then update test objs pos on rays pos -y 
    }

    public enum DirtLevel
    {
        Safe,
        UnSafe,
        Dirty
    }
}