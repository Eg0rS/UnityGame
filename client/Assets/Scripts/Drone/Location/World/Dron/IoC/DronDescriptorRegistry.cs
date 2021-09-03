using System.Collections.Generic;
using Drone.Location.World.Dron.Descriptor;

namespace Drone.Location.World.Dron.IoC
{
    public class DronDescriptorRegistry
    {
        private readonly List<DronDescriptor> _dronDescriptors;

        public DronDescriptorRegistry()
        {
            _dronDescriptors = new List<DronDescriptor>();
        }

        public List<DronDescriptor> DronDescriptors
        {
            get => _dronDescriptors;
        }
    }
}