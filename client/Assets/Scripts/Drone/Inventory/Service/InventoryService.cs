using System.Collections.Generic;
using AgkCommons.Event;
using Drone.Core.Filter;
using Drone.Inventory.Event;
using Drone.Inventory.Model;
using Drone.Shop.Descriptor;
using IoC.Attribute;

namespace Drone.Inventory.Service
{
    public class InventoryService : GameEventDispatcher, IInitable
    {
        [Inject]
        private InventoryRepository _inventoryRepository;

        [Inject]
        private ShopDescriptor _shopDescriptor;

        private InventoryModel _inventory;

        public void Init()
        {
            InitInventoryModel();
        }

        private bool HasInventoryModel()
        {
            return _inventoryRepository.Exists();
        }

        public void ResetInventory()
        {
            _inventory = new InventoryModel {
                    Items = new List<InventoryItemModel>()
            };
            InventoryItemModel defaultItem = new InventoryItemModel("dron1", InventoryItemTypeModel.DRON, 1);
            AddInventory(defaultItem);
            _inventoryRepository.Set(_inventory);
        }

        private void InitInventoryModel()
        {
            if (!HasInventoryModel()) {
                _inventory = new InventoryModel {
                        Items = new List<InventoryItemModel>()
                };
                InventoryItemModel defaultItem = new InventoryItemModel("dron1", InventoryItemTypeModel.DRON, 1);
                AddInventory(defaultItem);
                SaveInventoryModel(_inventory);
            } else {
                _inventory = _inventoryRepository.Require();
            }
        }

        public void AddInventory(InventoryItemModel item)
        {
            Inventory.Items.Add(item);
            SaveInventoryModel(_inventory);
            Dispatch(new InventoryEvent(InventoryEvent.UPDATED, item, item.Type));
        }

        private void SaveInventoryModel(InventoryModel model)
        {
            _inventoryRepository.Set(model);
        }

        public InventoryModel Inventory
        {
            get => _inventory;
        }
    }
}