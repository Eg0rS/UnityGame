using System.Collections.Generic;
using Drone.Levels.Model;

namespace Drone.LevelMap.Levels.Model
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress = new List<LevelProgress>();

        public List<LevelProgress> LevelsProgress
        {
            get { return _levelsProgress; }
            set { _levelsProgress = value; }
        }
    }
}