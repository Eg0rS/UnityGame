using System;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using Drone.Core.Filter;
using Drone.Location.World.SpeedBooster.Descriptor;
using Drone.Location.World.SpeedBooster.IoC;
using IoC.Attribute;

namespace Drone.Location.Service.Builder
{
    public class BoosterService : IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private BoosterDescriptorRegistry _boosterDescriptorRegistry;

        public void Init()
        {
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("boosters.booster")) {
                BoosterDescriptor boosterDescriptor = new BoosterDescriptor();
                boosterDescriptor.Configure(conf);
                _boosterDescriptorRegistry.BoosterDescriptors.Add(boosterDescriptor);
            }
        }

        public BoosterDescriptor GetDescriptorById(string id)
        {
            foreach (BoosterDescriptor boosterDescriptor in _boosterDescriptorRegistry.BoosterDescriptors) {
                if (boosterDescriptor.Id == id) {
                    return boosterDescriptor;
                }
            }
            throw new Exception("BoosterDescriptor not found");
        }
    }
}