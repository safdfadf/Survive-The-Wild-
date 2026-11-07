using System;

namespace SurviveTheWild.EventBus.Events
{
    public class CraftEvents
    {
        public Action<ObjSo> OnCraft;

        public Action<InventoryItem> onPlaceInCraftMenu;
        public Action<ObjSo, InventoryItem> onRemoveFromCraftMenu;

        public void ObjectCraft(ObjSo so)
        {
            OnCraft?.Invoke(so);
        }

        public void PlaceInCraftMenu(InventoryItem item)
        {
            onPlaceInCraftMenu?.Invoke(item);
        }

        public void RemoveFromCraftMenu(ObjSo so, InventoryItem item)
        {
            onRemoveFromCraftMenu(so, item);
        }
    }
}