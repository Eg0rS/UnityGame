using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Filter;
using Drone.Inventory.Service;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.IoC;
using Drone.Location.World.Dron.Model;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.Location.World.Dron.Service
{
    public class DronService : IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DronService>();
        private string _currentDronId;

        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;

        [Inject]
        private ResourceService _resourceService;
        [Inject]
        private InventoryService _inventoryService;

        public string CurrentDronId
        {
            get => _currentDronId;
        }

        public void Init()
        {
            if (_dronDescriptorRegistry.DronDescriptors.Count != 0) {
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
                DronDescriptor dronDescriptor = new DronDescriptor();
                dronDescriptor.Configure(conf);
                _dronDescriptorRegistry.DronDescriptors.Add(dronDescriptor);
            }
            _logger.Debug("[DronService] Теперь количество элементов в _dronDescriptorRegistry.DronDescriptors = "
                          + _dronDescriptorRegistry.DronDescriptors.Count);
        }

        [NotNull]
        public DroneModel GetDronById(string dronId)
        {
            return new DroneModel(_dronDescriptorRegistry.DronDescriptors.Find(it => it.Id.Equals(dronId)));
        }
    }
}