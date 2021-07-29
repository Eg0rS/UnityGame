using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Event;
using DronDonDon.Inventory.Model;
using IoC.Attribute;

namespace DronDonDon.Inventory.Service
{
    public class InventoryService : GameEventDispatcher
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<InventoryService>();

        [Inject] 
        private InventoryRepository _inventoryRepository;

        public InventoryModel _inventory;
        
        public void Init()
        {
            InitInventoryModel();
        }

        public bool HasInventoryModel()
        {
            return (_inventoryRepository.Get() != null);
        }

        private void InitInventoryModel()
        {
            if (!HasInventoryModel())
            {
                _inventory = new InventoryModel {Items = new List<InventoryItemModel>()};
            }
            
        }
    }
}