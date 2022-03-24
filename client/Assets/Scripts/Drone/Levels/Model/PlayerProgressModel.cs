using System.Collections.Generic;
using Drone.Random;

namespace Drone.Levels.Model
{
    public class PlayerProgressModel
    {
        private List<LevelProgress> _levelsProgress;
        private uint _seed;
        private int _respawnCount;

        public List<LevelProgress> LevelsProgress
        {
            get { return _levelsProgress; }
            set { _levelsProgress = value; }
        }

        public uint Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }
        public int RespawnCount
        {
            get { return _respawnCount; }
            set { _respawnCount = value; }
        }

        public PlayerProgressModel()
        {
            Seed = RandomSeedGenerator.Crypto();
            LevelsProgress = new List<LevelProgress>();
            RespawnCount = 0;
        }
    }
}