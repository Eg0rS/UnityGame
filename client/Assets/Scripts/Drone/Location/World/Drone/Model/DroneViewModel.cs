using Drone.Location.World.Drone.Descriptor;

namespace Drone.Location.World.Drone.Model
{
    public class DroneViewModel
    {
        private DroneDescriptor _droneDescriptor;

        public DroneDescriptor DroneDescriptor
        {
            get => _droneDescriptor;
            set => _droneDescriptor = value;
        }
    }
}