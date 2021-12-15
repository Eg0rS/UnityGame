using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Inventory.Service;
using Drone.Location.Service.Control.Drone.Descriptor;
using Drone.Location.Service.Control.Drone.IoC;
using Drone.Location.Service.Control.Drone.Model;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.Location.Service.Control.Drone.Service
{
    public class DroneService : IConfigurable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DroneService>();

        [Inject]
        private DroneDescriptorRegistry _droneDescriptorRegistry;

        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private InventoryService _inventoryService;

        public void Configure()
        {
            if (_droneDescriptorRegistry.DroneDescriptors.Count != 0) {
                return;
            }
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
        }

        [NotNull]
        public DroneModel GetDronById(string dronId)
        {
            return new DroneModel(_droneDescriptorRegistry.DroneDescriptors.Find(it => it.Id.Equals(dronId)));
        }
    }
}