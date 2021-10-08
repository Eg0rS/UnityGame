using Drone.Location.World.Drone.Descriptor;

namespace Drone.Location.World.Drone.Model
{
    public class DroneViewModel
    {
        private DronDescriptor _droneDescriptor;

        public DronDescriptor DroneDescriptor
        {
            get => _droneDescriptor;
            set => _droneDescriptor = value;
        }
    }
}