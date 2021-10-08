using System.Collections.Generic;
using AgkCommons.Event;
using Drone.Core.Filter;
using Drone.Inventory.Event;
using Drone.Inventory.Model;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.IoC;
using Drone.Location.World.Dron.Service;
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

        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;

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
            _inventory = new InventoryModel {
                    Items = new List<InventoryItemModel>()
            };
        }

        public void AddAllDrones()
        {
            foreach (DronDescriptor descriptor in _dronDescriptorRegistry.DronDescriptors) {
                AddInventory(new InventoryItemModel(descriptor.Id, InventoryItemTypeModel.DRON, 1));
            }
        }
        // Комментарий на время теста всех дронов
        // private void InitInventoryModel()
        // {
        //     if (!HasInventoryModel()) {
        //         _inventory = new InventoryModel {
        //                 Items = new List<InventoryItemModel>()
        //         };
        //         InventoryItemModel defaultItem = new InventoryItemModel("dron1", InventoryItemTypeModel.DRON, 1);
        //         AddInventory(defaultItem);
        //         SaveInventoryModel(_inventory);
        //     } else {
        //         _inventory = _inventoryRepository.Require();
        //     }
        // }

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