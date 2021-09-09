using System.Collections.Generic;
using DeliveryRush.LevelMap.Regions.Descriptor;

namespace DeliveryRush.LevelMap.Regions.IoC
{
    public class RegionDescriptorRegistry
    {
        private List<RegionDescriptor> _regionDescriptors;

        public RegionDescriptorRegistry()
        {
            _regionDescriptors = new List<RegionDescriptor>();
        }

        public List<RegionDescriptor> RegionDescriptors
        {
            get { return _regionDescriptors; }
        }
    }
}