using AgkCommons.Repository;
using Drone.Inventory.Model;

namespace Drone.Inventory.Service
{
    public class InventoryRepository : LocalPrefsSingleRepository<InventoryModel>
    {
        public InventoryRepository() : base("inventoryRepository")
        {
        }
    }
}