namespace Drone.Levels.Model
{
    public class LevelProgress
    {
        private string _id;
        private int _countChips;
        private string _levelVersion;
        private int _randomSeed;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string LevelVersion
        {
            get { return _levelVersion; }
            set { _levelVersion = value; }
        }
        public int RandomSeed
        {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        public int CountChips
        {
            get { return _countChips; }
            set { _countChips = value; }
        }
    }
}