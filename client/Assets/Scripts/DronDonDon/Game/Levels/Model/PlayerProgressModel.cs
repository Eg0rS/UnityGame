using System.Collections.Generic;
using DronDonDon.Game.Levels.Model;
using IoC.Attribute;
using NUnit.Framework;

namespace DronDonDon.Game.Levels
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress;
        
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