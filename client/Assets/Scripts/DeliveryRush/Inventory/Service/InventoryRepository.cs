using AgkCommons.Repository;
using DeliveryRush.Inventory.Model;

namespace DeliveryRush.Inventory.Service
{
    public class InventoryRepository : LocalPrefsSingleRepository<InventoryModel>
    {
        public InventoryRepository() : base("inventoryRepository")
        {
        }
    }
}