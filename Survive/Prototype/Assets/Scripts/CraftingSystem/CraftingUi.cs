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
            if (resourceInventory.HeldItem == null)
            {
//                print("pick up item from  slot");
                resourceInventory.HeldItem = slot.occupiedItem;

                slot.occupiedItem.rect.SetParent(resourceInventory.transform);
                slot.occupiedItem.rect.SetAsLastSibling();
                global::EventBus.OnUnCraftResource.Invoke(slot.occupiedItem.so, slot.occupiedItem);
                slot.occupiedItem.IsInCraftingList = false;
                slot.occupiedItem = null;
                // tell crafting handler to remove this from ingredient 
            }
            else
            {
                //              print("drop item in slot");
                var item = resourceInventory.HeldItem;
                resourceInventory.HeldItem = null;
                global::EventBus.OnCraftResource.Invoke(item.so, item);
                slot.occupiedItem = item;
                item.IsInCraftingList = true;
                item.rect.SetParent(slot.rect);
                item.rect.localPosition = Vector3.zero;
                item.rect.localRotation = Quaternion.identity;
                item.slot = slot;
            }
        }

        public void PlaceItemInSlot(InventoryItem item)
        {
            foreach (var slot in craftingSlots)
            {
                if (slot.occupiedItem != null) continue;
                slot.occupiedItem = item;
                item.rect.SetParent(slot.rect);
                item.rect.localPosition = Vector3.zero;
                item.rect.localRotation = Quaternion.identity;
                item.slot = slot;
            }
        }
    }
}