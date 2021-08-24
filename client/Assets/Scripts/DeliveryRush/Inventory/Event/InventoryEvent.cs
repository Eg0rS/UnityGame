using AgkCommons.Event;
using DeliveryRush.Inventory.Model;

namespace DeliveryRush.Inventory.Event
{
    public class InventoryEvent : GameEvent
    {
        public const string UPDATED = "UpdateInventory";

        public InventoryItemModel Item { get; }
        
        public InventoryItemTypeModel InventoryType { get; }

        public InventoryEvent(string name, InventoryItemModel item, InventoryItemTypeModel inventoryType) : base(name)
        {
            Item = item;
            InventoryType = inventoryType;
        }
    }
}