using System;
using DefaultNamespace.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FoodSystem
{
    public class Coconut : Food
    {
        private bool isOnGround;
        private Rigidbody _rigidbody;
        private float fallChance = .1f; // change it t0 .8f
        [SerializeField] private ObjSo peeledCoco;

        protected override void Awake()
        {
            base.Awake();
            canCookMe = false;
        }

        private void OnEnable()
        {
            EventBus.OnHourChanged += Fall;
        }

        private void OnDisable()
        {
            EventBus.OnHourChanged -= Fall;
        }

        public override void Harvest()
        {
            for (int i = 0; i < So.amount; i++)
            {
                SpawnHarvestCoco();
            }
        }

        private void SpawnHarvestCoco()
        {
            GameObject o = GlobalPool.instance.Get(peeledCoco.prefab, transform.position);
            Food food = o.GetComponent<Food>();
            food.Initialize(peeledCoco);
            if (InventoryItem != null)
                Destroy(InventoryItem.gameObject);
            EventManager.Instance.reseourceEvent.GatherResource(o);
            Gm = null;
            base.Harvest();
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isOnGround = true;
            }
        }

        private void Fall(int hour)
        {
            if (isOnGround && !gameObject.activeInHierarchy) return;
            if ((Random.value < fallChance)) return;
            if (_rigidbody == null)
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            if (_rigidbody != null)
                _rigidbody.isKinematic = false;
        }
    }
}