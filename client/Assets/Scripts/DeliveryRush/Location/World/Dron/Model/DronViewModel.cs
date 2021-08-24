using DeliveryRush.Location.World.Dron.Descriptor;

namespace DeliveryRush.Location.World.Dron.Model
{
    public class DronViewModel
    {
        private DronDescriptor _dronDescriptor;

        public DronDescriptor DronDescriptor
        {
            get => _dronDescriptor;
            set => _dronDescriptor = value;
        }
    }
}