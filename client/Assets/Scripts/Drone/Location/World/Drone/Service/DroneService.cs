using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Filter;
using Drone.Inventory.Service;
using Drone.Location.World.Drone.Descriptor;
using Drone.Location.World.Drone.IoC;
using Drone.Location.World.Drone.Model;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.Location.World.Drone.Service
{
    public class DroneService : IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DroneService>();

        [Inject]
        private DroneDescriptorRegistry _droneDescriptorRegistry;

        [Inject]
        private ResourceService _resourceService;
        
        [Inject]
        private InventoryService _inventoryService;
        
        public void Init()
        {
            if (_droneDescriptorRegistry.DroneDescriptors.Count != 0) {
                return;
            }
            _logger.Debug("[DronService] В _dronDescriptorRegistry.DronDescriptors пусто ...");
            _resourceService.LoadConfiguration("Configs/drons@embeded")
                            .Then(x => {
                                OnConfigLoaded(x);
                                _inventoryService.AddAllDrones();
                            });
        }

        private void OnConfigLoaded(Configuration config)
        {
            foreach (Configuration conf in config.GetList<Configuration>("drons.dron")) {
                DroneDescriptor dronDescriptor = new DroneDescriptor();
                dronDescriptor.Configure(conf);
                _droneDescriptorRegistry.DroneDescriptors.Add(dronDescriptor);
            }
            _logger.Debug("[DronService] Теперь количество элементов в _dronDescriptorRegistry.DronDescriptors = "
                          + _droneDescriptorRegistry.DroneDescriptors.Count);
        }

        [NotNull]
        public DroneModel GetDronById(string dronId)
        {
            return new DroneModel(_droneDescriptorRegistry.DroneDescriptors.Find(it => it.Id.Equals(dronId)));
        }
    }
}