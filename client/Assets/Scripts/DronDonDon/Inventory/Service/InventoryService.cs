using System;
using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Event;
using DronDonDon.Inventory.Model;
using IoC.Attribute;
using DronDonDon.Core.Filter;
using DronDonDon.Inventory.Event;
using DronDonDon.Shop.Descriptor;

namespace DronDonDon.Inventory.Service
{
    public class InventoryService : GameEventDispatcher, IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<InventoryService>();

        [Inject] 
        private InventoryRepository _inventoryRepository;

        [Inject]
        private ShopDescriptor _shopDescriptor;
        
        private InventoryModel _inventory;
        
        
        public void Init()
        {
            InitInventoryModel();
        }

        public bool HasInventoryModel()
        {
            return (_inventoryRepository.Get() != null);
        }

        public void ResetInventory()
        {
            _inventory = new InventoryModel {Items = new List<InventoryItemModel>()};
            _inventoryRepository.Set(_inventory);
        }
        private void InitInventoryModel()
        {
            if (!HasInventoryModel())
            {
                _inventory = new InventoryModel {Items = new List<InventoryItemModel>()};
            }
            else
            {
                _inventory = _inventoryRepository.Require();
            }
        }
        public void AddInventory(string itemId)
        {
            InventoryItemModel item = new InventoryItemModel(); 
            item = Inventory.GetItem(itemId); 
            if (item != null) { 
                return;
            }
            ShopItemDescriptor shopItemDescriptor = _shopDescriptor.RequireShopItem(itemId);
            
            item = new InventoryItemModel(itemId, shopItemDescriptor.Type, 1); 
            Inventory.Items.Add(item);
            _inventoryRepository.Set(Inventory);
            Dispatch(new InventoryEvent(InventoryEvent.UPDATED, item, shopItemDescriptor.Type));
        }
        
        
        public InventoryModel Inventory
        {
            get {return _inventory;}
        }
    }
}