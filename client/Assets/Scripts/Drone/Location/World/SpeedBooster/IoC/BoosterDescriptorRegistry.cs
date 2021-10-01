using System.Collections.Generic;
using Drone.Location.World.SpeedBooster.Descriptor;

namespace Drone.Location.World.SpeedBooster.IoC
{
    public class BoosterDescriptorRegistry
    {
        private readonly List<BoosterDescriptor> _boosterDescriptors;

        public BoosterDescriptorRegistry()
        {
            _boosterDescriptors = new List<BoosterDescriptor>();
        }

        public List<BoosterDescriptor> BoosterDescriptors
        {
            get => _boosterDescriptors;
        }
    }
}