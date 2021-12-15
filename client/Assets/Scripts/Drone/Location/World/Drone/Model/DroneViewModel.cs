using Drone.Location.Service.Control.Drone.Descriptor;

namespace Drone.Location.Service.Control.Drone.Model
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