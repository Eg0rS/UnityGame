using System.Collections.Generic;
using Drone.Location.Service.Control.Drone.Descriptor;

namespace Drone.Location.Service.Control.Drone.IoC
{
    public class DroneDescriptorRegistry
    {
        private readonly List<DroneDescriptor> _droneDescriptors;

        public DroneDescriptorRegistry()
        {
            _droneDescriptors = new List<DroneDescriptor>();
        }

        public List<DroneDescriptor> DroneDescriptors
        {
            get => _droneDescriptors;
        }
    }
}