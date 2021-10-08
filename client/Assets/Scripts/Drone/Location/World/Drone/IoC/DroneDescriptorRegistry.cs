using System.Collections.Generic;
using Drone.Location.World.Drone.Descriptor;

namespace Drone.Location.World.Drone.IoC
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