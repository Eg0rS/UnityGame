using Drone.Location.World.Dron.Descriptor;

namespace Drone.Location.World.Dron.Model
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