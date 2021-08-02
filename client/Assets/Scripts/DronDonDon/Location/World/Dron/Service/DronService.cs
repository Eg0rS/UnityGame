using System;
using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using DronDonDon.Core.Service;
using DronDonDon.Location.World.Dron.Descriptor;
using DronDonDon.Location.World.Dron.IoC;
using IoC.Attribute;

namespace DronDonDon.Location.World.Dron.Service
{
    public class DronService 
    {
        [Inject]
        private DronRepository _dronRepository;

        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;
        
        [Inject]
        private ResourceService _resourceService;

        public void Init()
        {
            if (_dronDescriptorRegistry.DronDescriptors.Count == 0)
            {
                _resourceService.LoadConfiguration("Configs/drons@embeded", OnConfigLoaded);
            }
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration conf in config.GetList<Configuration>("drons.dron"))
            {
                DronDescriptor dronDescriptor = new DronDescriptor();
                dronDescriptor.Configure(conf);
                _dronDescriptorRegistry.DronDescriptors.Add(dronDescriptor);
            }
        }
        
        
        
    }
}