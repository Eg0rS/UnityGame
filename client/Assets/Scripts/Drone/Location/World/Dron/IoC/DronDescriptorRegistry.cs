using System.Collections.Generic;
using Drone.Location.World.Dron.Descriptor;

namespace Drone.Location.World.Dron.IoC
{
    public class DronDescriptorRegistry
    {
        private readonly List<DroneDescriptor> _dronDescriptors;

        public DronDescriptorRegistry()
        {
            _dronDescriptors = new List<DroneDescriptor>();
        }

        public List<DroneDescriptor> DronDescriptors
        {
            get => _dronDescriptors;
        }
    }
}