using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using Drone.Core.Service;
using Drone.Descriptor;
using Drone.Location.Model;
using IoC.Attribute;
using JetBrains.Annotations;

namespace Drone.Location.Service
{
    public class BoosterService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private ResourceService _resourceService;
        private Dictionary<string, BoosterDescriptor> _boosterDescriptors;

        public void Init()
        {
            _boosterDescriptors = new Dictionary<string, BoosterDescriptor>();
            _resourceService.LoadConfiguration("Configs/boosters@embeded", OnConfigLoaded);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("boosters.booster")) {
                BoosterDescriptor boosterDescriptor = new BoosterDescriptor();
                boosterDescriptor.Configure(conf);
                _boosterDescriptors.Add(boosterDescriptor.Type, boosterDescriptor);
            }
        }

        [NotNull]
        public BoosterDescriptor GetDescriptorByType(WorldObjectType objectType)
        {
            string type = Enum.GetName(typeof(WorldObjectType), objectType);
            return _boosterDescriptors[type];
        }
    }
}