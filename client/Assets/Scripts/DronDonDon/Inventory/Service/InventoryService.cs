using System;
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
            else
            {
                InventoryModel _inventory = _inventoryRepository.Require();
            }
        }
        /*public void AddInventory(string itemId)
        {
            InventoryItemModel item = Inventory.GetItem(itemId); 
            if (item != null) { 
                return;
            }
           // ShopItemDescriptor shopItemDescriptor = _shopDescriptor.RequireShopItem(itemId);
            List<InventoryItemModel> inventoryItems = Inventory.Items;
            item = new InventoryItemModel(itemId, shopItemDescriptor.Type, 1); 
            inventoryItems.Add(item);
            Dispatch(new InventoryEvent(InventoryEvent.UPDATED, item, shopItemDescriptor.Type));
        }*/
        public InventoryModel Inventory
        {
            get
            {
                if (_inventory == null) {
                    throw new NullReferenceException("Inventory model not initialized");
                }
                return _inventory;
            }
        }
    }
}