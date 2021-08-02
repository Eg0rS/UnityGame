using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using DronDonDon.Core.Filter;
using DronDonDon.Location.World.Dron.Descriptor;
using DronDonDon.Location.World.Dron.IoC;
using DronDonDon.Location.World.Dron.Model;
using IoC.Attribute;

namespace DronDonDon.Location.World.Dron.Service
{
    public class DronService : IInitable
    {
        [Inject]
        private DronDescriptorRegistry _dronDescriptorRegistry;
        
        [Inject]
        private ResourceService _resourceService;
        
        private List<DronViewModel> _dronViewModels = new List<DronViewModel>();

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

        public List<DronViewModel> getAllDrons()
        {
            _dronViewModels = new List<DronViewModel>();
            foreach (DronDescriptor item in _dronDescriptorRegistry.DronDescriptors)
            {
                DronViewModel dronViewModel = new DronViewModel();
                dronViewModel.DronDescriptor = item;
                _dronViewModels.Add(dronViewModel);
            }
            return _dronViewModels;
        }
    }
}