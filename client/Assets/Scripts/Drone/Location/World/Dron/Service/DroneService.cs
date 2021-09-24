using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Filter;
using Drone.Location.World.Dron.Descriptor;
using Drone.Location.World.Dron.IoC;
using Drone.Location.World.Dron.Model;
using IoC.Attribute;

namespace Drone.Location.World.Dron.Service
{
    public class DronService : IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DronService>();

        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;

        [Inject]
        private ResourceService _resourceService;
        
        public void Init()
        {
            if (_dronDescriptorRegistry.DronDescriptors.Count == 0) {
                _logger.Debug("[DronService] В _dronDescriptorRegistry.DronDescriptors пусто ...");
                _resourceService.LoadConfiguration("Configs/drons@embeded", OnConfigLoaded);
            }
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("drons.dron")) {
                DroneDescriptor droneDescriptor = new DroneDescriptor();
                droneDescriptor.Configure(conf);
                _dronDescriptorRegistry.DronDescriptors.Add(droneDescriptor);
            }
            _logger.Debug("[DronService] Теперь количество элементов в _dronDescriptorRegistry.DronDescriptors = "
                          + _dronDescriptorRegistry.DronDescriptors.Count);
        }

        public DroneViewModel GetDroneById(string droneId)
        {
            DroneViewModel droneViewModel = new DroneViewModel();
            droneViewModel.DroneDescriptor = _dronDescriptorRegistry.DronDescriptors.Find(it => it.Id.Equals(droneId));
            return droneViewModel;
        }
    }
}