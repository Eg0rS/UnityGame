using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Filter;
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
        private string _currentDronId;

        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;

        [Inject]
        private ResourceService _resourceService;

        public string CurrentDronId
        {
            get => _currentDronId;
        }

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