using System.Collections.Generic;

namespace Drone.LevelMap.Levels.Model
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress = new List<LevelProgress>();

        private string _currentRegionId = "1";

        public List<LevelProgress> LevelsProgress
        {
            get { return _levelsProgress; }
            set { _levelsProgress = value; }
        }

        public string CurrentRegionId
        {
            get { return _currentRegionId; }
            set { _currentRegionId = value; }
        }

    }
}