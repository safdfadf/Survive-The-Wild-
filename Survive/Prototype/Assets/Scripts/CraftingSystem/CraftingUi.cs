using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace SurviveTheWild.CraftingSystem
{
    public class CraftingUi : MonoBehaviour
    {
        [SerializeField] private Slot slotPrefab;
        [SerializeField] private Transform parent;

        private ResourceInventory resourceInventory;
        private List<Slot> craftingSlots = new();
        [SerializeField] private int slotCount = 8;

        private void Awake()
        {
            for (var i = 0; i < slotCount; i++)
            {
                var slot = Instantiate(slotPrefab, parent);
                slot.SlotType = SlotType.CraftingSLot;
                craftingSlots.Add(slot);
                slot.storeCapacity = 1;
            }
        }

        private void OnEnable()
        {
            SceneRoot.Instance.EventManager.CraftEvents.onPlaceInCraftMenu += PlaceItemInSlot;
        }

        private void OnDisable()
        {
            SceneRoot.Instance.EventManager.CraftEvents.onPlaceInCraftMenu -= PlaceItemInSlot;
        }

        private void Start()
        {
            resourceInventory = PlayerRepository.instance.resourceInventory;
        }

        public void OnClick(Slot slot)
        {
            if (resourceInventory.HeldItem == null) // pick up 
            {
                PickUp(slot);
            }
            else // drop
            {
                Drop(slot);
            }
        }

        private void PickUp(Slot slot)
        {
            InventoryItem item = slot.occupiedItem[0];
            resourceInventory.HeldItem = item;
            item.rect.SetParent(resourceInventory.transform);
            item.rect.SetAsLastSibling();
            global::EventBus.OnUnCraftResource.Invoke(slot.occupiedItem[0].so, item);
            item.IsInCraftingList = false;
            slot.occupiedItem = null;
        }

        private void Drop(Slot slot)
        {
            var item = resourceInventory.HeldItem;
            resourceInventory.HeldItem = null;
            global::EventBus.OnCraftResource.Invoke(item.so, item);
            slot.occupiedItem.Add(item);
            item.IsInCraftingList = true;
            item.rect.SetParent(slot.rect);
            item.rect.localPosition = Vector3.zero;
            item.rect.localRotation = Quaternion.identity;
            item.slot = slot;
        }

        public void PlaceItemInSlot(InventoryItem item)
        {
            foreach (var slot in craftingSlots)
            {
                if (!slot.CanOccupy()) continue;
                slot.occupiedItem.Add(item);
                item.rect.SetParent(slot.rect);
                item.rect.localPosition = Vector3.zero;
                item.rect.localRotation = Quaternion.identity;
                item.slot = slot;
            }
        }
    }
}