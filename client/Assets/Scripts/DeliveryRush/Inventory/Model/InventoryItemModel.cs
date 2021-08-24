namespace DeliveryRush.Inventory.Model
{
    public class InventoryItemModel
    {
        public string Id { get; }
        public InventoryItemTypeModel Type { get; }

        private int Count { get; }

        public InventoryItemModel(string itemId, InventoryItemTypeModel type, int count)
        {
            Id = itemId;
            Type = type;
            Count = count;
        }
    }
}