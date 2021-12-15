using System.Collections.Generic;
using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Inventory.Event;
using Drone.Inventory.Model;
using Drone.Location.Service.Control.Drone.Descriptor;
using Drone.Location.Service.Control.Drone.IoC;
using IoC.Attribute;

namespace Drone.Inventory.Service
{
    public class InventoryService : GameEventDispatcher, IConfigurable
    {
        [Inject]
        private InventoryRepository _inventoryRepository;
        
        [Inject]
        private DroneDescriptorRegistry _droneDescriptorRegistry;
        
        private InventoryModel _inventory;

        public void Configure()
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
            foreach (DroneDescriptor descriptor in _droneDescriptorRegistry.DroneDescriptors) {
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