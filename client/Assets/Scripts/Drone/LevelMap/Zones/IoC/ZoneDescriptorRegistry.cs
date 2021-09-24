using System.Collections.Generic;
using Drone.LevelMap.Zones.Descriptor;

namespace Drone.LevelMap.Zones.IoC
{
    public class ZoneDescriptorRegistry
    {
        private List<ZoneDescriptor> _zoneDescriptors;

        public ZoneDescriptorRegistry()
        {
            _zoneDescriptors = new List<ZoneDescriptor>();
        }

        public List<ZoneDescriptor> ZoneDescriptors
        {
            get { return _zoneDescriptors; }
        }
    }
}