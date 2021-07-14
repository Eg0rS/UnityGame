using AgkUI.Binding.Attributes;
using DronDonDon.MainMenu.UI.Settings.Model;
using IoC.Attribute;

namespace DronDonDon.MainMenu.UI.Settings.Service
{
    public class SettingsService
    {
       [Inject]
        private SettingsRepository _settingsRepository;
        
        public void UpdateSettings()
        {
            SettingsModel settingsModel = RequireSettingsModel();
            SetMusicMute(settingsModel.IsMusicMute);
        }

        public bool HasSettingsModel()
        {
            return (_settingsRepository.Get() != null);
        }

        public SettingsModel RequireSettingsModel()
        {
            return _settingsRepository.Require();
        }

        [UICreated]
        public void InitSettings()
        {
            if (!HasSettingsModel()) {
                SettingsModel settingsModel = new SettingsModel();
                settingsModel.IsMusicMute = true;
                settingsModel.IsSoundMute = true;
                _settingsRepository.Set(settingsModel);
            }

            UpdateSettings();
        }
        
        public bool GetMusicMute()
        {
            return RequireSettingsModel().IsMusicMute;
        }

        public void SetMusicMute(bool isMute)
        {
          //  AudioListener.volume = isMute ? 1f : 0f;
            SettingsModel settingsModel = RequireSettingsModel();
            settingsModel.IsMusicMute = isMute;
            _settingsRepository.Set(settingsModel);
        }
        
        public bool GetSoundMute()
        {
            return RequireSettingsModel().IsSoundMute;
        }

        public void SetSoundMute(bool isMute)
        {
          //  AudioListener.volume = isMute ? 1f : 0f;
            SettingsModel settingsModel = RequireSettingsModel();
            settingsModel.IsSoundMute = isMute;
            _settingsRepository.Set(settingsModel);
        }
    }
}