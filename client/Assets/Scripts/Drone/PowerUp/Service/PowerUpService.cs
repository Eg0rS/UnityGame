using System.Collections.Generic;
using System.Data;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.PowerUp.Descriptor;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.PowerUp.Service
{
    public class PowerUpService : IConfigurable
    {
        [Inject]
        private ResourceService _resourceService;

        private Dictionary<PowerUpType, PowerUpDescriptor> _powerUpDescriptors;

        public void Configure()
        {
            _powerUpDescriptors = new Dictionary<PowerUpType, PowerUpDescriptor>();
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
        }

        private void OnConfigLoaded(Configuration configuration, object[] loadparameters)
        {
            foreach (Configuration config in configuration.GetList<Configuration>("boosters.booster")) {
                PowerUpDescriptor descriptor = new PowerUpDescriptor();
                descriptor.Configure(config);
                _powerUpDescriptors.Add(descriptor.Type, descriptor);
            }
        }

        [NotNull]
        public PowerUpDescriptor GetDescriptorByType(PowerUpType powerUpType)
        {
            PowerUpDescriptor descriptor = _powerUpDescriptors[powerUpType];
            return descriptor == null ? throw new NoNullAllowedException($"Descriptor {powerUpType} is not found") : descriptor;
        }
    }
}