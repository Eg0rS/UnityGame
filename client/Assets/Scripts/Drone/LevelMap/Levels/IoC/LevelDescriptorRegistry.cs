using System.Collections.Generic;
using Drone.LevelMap.Levels.Descriptor;

namespace Drone.LevelMap.Levels.IoC
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