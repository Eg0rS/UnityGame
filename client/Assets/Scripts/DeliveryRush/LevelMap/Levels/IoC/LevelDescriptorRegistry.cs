using System.Collections.Generic;
using DeliveryRush.LevelMap.Levels.Descriptor;

namespace DeliveryRush.LevelMap.Levels.IoC
{
    public class LevelDescriptorRegistry
    {
        private List<LevelDescriptor> _levelDescriptors;

        public LevelDescriptorRegistry()
        {
            _levelDescriptors = new List<LevelDescriptor>();
        }

        public List<LevelDescriptor> LevelDescriptors
        {
            get { return _levelDescriptors; }
        }
    }
}