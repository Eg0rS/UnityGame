using Drone.Location.World.Dron.Descriptor;

namespace Drone.Location.World.Dron.Model
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