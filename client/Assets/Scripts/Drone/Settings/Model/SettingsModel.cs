namespace Drone.Settings.Model
{
    public class SettingsModel
    {
        private bool _isMusicMute;
        private bool _isSoundMute;
        private int _seed;

        public int Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public bool IsSoundMute
        {
            get { return _isSoundMute; }
            set { _isSoundMute = value; }
        }
        public bool IsMusicMute
        {
            get { return _isMusicMute; }
            set { _isMusicMute = value; }
        }
    }
}