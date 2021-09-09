using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Drone.Inventory.Model
{
    [UsedImplicitly]
    public class InventoryModel
    {
        public List<InventoryItemModel> Items { get; set; }

        [CanBeNull]
        public InventoryItemModel GetItem(string itemId)
        {
            return Items.FirstOrDefault(x => x.Id == itemId);
        }

        public bool HasItem(string itemId)
        {
            InventoryItemModel item = Items.FirstOrDefault(x => x.Id == itemId);
            return item != null;
        }
    }
}