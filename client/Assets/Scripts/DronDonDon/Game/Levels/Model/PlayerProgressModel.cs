using System.Collections.Generic;

namespace DronDonDon.Game.Levels.Model
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress;
        private string _nextLevelId;

        public string NextLevel
        {
            get => _nextLevelId;
            set => _nextLevelId = value;
        }

        public List<LevelProgress> LevelsProgress
        {
            get { return _levelsProgress; }
            set { _levelsProgress = value; }
        }
        
        public PlayerProgressModel()
        {
            _levelsProgress = new List<LevelProgress>();
        }
    }
}