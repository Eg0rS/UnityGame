using AgkUI.Binding.Attributes;
using DronDonDon.Settings.Model;
using DronDonDon.Billing.Service;

using IoC.Attribute;

namespace DronDonDon.Settings.Service
{
    public class SettingsService
    {
        [Inject]
        private SettingsRepository _settingsRepository;
        
        [Inject] 
        private BillingService _billingService;
        
        public void UpdateSettings()
        {
            SettingsModel settingsModel = RequireSettingsModel();
            SetMusicMute(settingsModel.IsMusicMute);
            SetSoundMute(settingsModel.IsSoundMute);
        }

        public bool HasSettingsModel()
        {
            return (_settingsRepository.Get() != null);
        }

        public SettingsModel RequireSettingsModel()
        {
            SettingsModel model = _settingsRepository.Get();
            if (model == null)
            {
                InitSettings();
            }
            return _settingsRepository.Require();
        }
        
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

        public void ResetAllProgress()
        {
            _billingService.SetCreditsCount(0);
        }
    }
}