using Drone.Location.World.Player.Descriptor;

namespace Drone.Location.World.Player.Model
{
    public class DroneModel
    {
        public float mobility;
        private DroneDescriptor _droneDescriptor;

        public DroneModel(DroneDescriptor descriptor)
        {
            mobility = descriptor.Mobility;
            DroneDescriptor = descriptor;
        }

        public DroneDescriptor DroneDescriptor
        {
            get { return _droneDescriptor; }
            private set { _droneDescriptor = value; }
        }
    }
}