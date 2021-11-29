using Drone.Levels.Descriptor;

namespace Drone.Levels.Model
{
    public class LevelViewModel
    {
        private LevelProgress _levelProgress;
        private LevelDescriptor _levelDescriptor;

        public LevelProgress LevelProgress
        {
            get { return _levelProgress; }
            set { _levelProgress = value; }
        }

        public LevelDescriptor LevelDescriptor
        {
            get { return _levelDescriptor; }
            set { _levelDescriptor = value; }
        }
    }
}