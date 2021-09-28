namespace Drone.Settings.Model
{
    public class SettingsModel
    {
        private bool _isMusicMute;
        private bool _isSoundMute;
        private bool _isSwipe;

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
        public bool IsSwipe
        {
            get { return _isSwipe; }
            set { _isSwipe = value; }
        }
    }
}