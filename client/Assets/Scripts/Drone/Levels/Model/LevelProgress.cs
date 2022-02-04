using System.Collections.Generic;
using Drone.Levels.Descriptor;

namespace Drone.Levels.Model
{
    public class LevelProgress
    {
        private string _id;
        private int _countStars;
        private int _countChips;
        private Dictionary<string, bool> _levelTasks;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int CountStars
        {
            get { return _countStars; }
            set { _countStars = value; }
        }

        public int CountChips
        {
            get { return _countChips; }
            set { _countChips = value; }
        }

        public Dictionary<string, bool> LevelTasks
        {
            get { return _levelTasks; }
            set { _levelTasks = value; }
        }
    }
}