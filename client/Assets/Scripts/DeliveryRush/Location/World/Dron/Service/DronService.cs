﻿using System.Collections.Generic;
using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using DeliveryRush.Core.Filter;
using DeliveryRush.Location.World.Dron.Descriptor;
using DeliveryRush.Location.World.Dron.IoC;
using DeliveryRush.Location.World.Dron.Model;
using IoC.Attribute;

namespace DeliveryRush.Location.World.Dron.Service
{
    public class DronService : IInitable
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<DronService>();
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

        public DronViewModel GetDronById(string dronId)
        {
            DronViewModel dronViewModel = new DronViewModel();
            dronViewModel.DronDescriptor = _dronDescriptorRegistry.DronDescriptors.Find(it => it.Id.Equals(dronId));
            return dronViewModel;
        }
    }
}