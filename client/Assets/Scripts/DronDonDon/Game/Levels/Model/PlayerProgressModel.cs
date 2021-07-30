using System.Collections.Generic;

namespace DronDonDon.Game.Levels.Model
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress;
        private string _currentLevel;

        public string CurrentLevel
        {
            get => _currentLevel;
            set => _currentLevel = value;
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