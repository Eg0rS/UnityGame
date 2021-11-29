using Drone.Billing.Service;
using Drone.Core.Audio.Service;
using Drone.Core.Filter;
using Drone.Inventory.Service;
using Drone.Levels.Service;
using Drone.Location.Service;
using Drone.Settings.Model;
using IoC.Attribute;

namespace Drone.Settings.Service
{
    public class SettingsService : IInitable
    {
        [Inject]
        private SettingsRepository _settingsRepository;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private LevelService _levelService;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private AudioService _audioService;
        
        private void UpdateSettings()
        {
            SettingsModel settingsModel = RequireSettingsModel();
            SetMusicMute(settingsModel.IsMusicMute);
            SetSoundMute(settingsModel.IsSoundMute);
        }

        private bool HasSettingsModel()
        {
            return (_settingsRepository.Get() != null);
        }

        private SettingsModel RequireSettingsModel()
        {
            SettingsModel model = _settingsRepository.Get();
            if (model == null) {
                Init();
            }
            return _settingsRepository.Require();
        }

        public void Init()
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
            _audioService.MusicMute = !isMute;
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
            _audioService.SoundMute = !isMute;
            SettingsModel settingsModel = RequireSettingsModel();
            settingsModel.IsSoundMute = isMute;
            _settingsRepository.Set(settingsModel);
        }

        public void ResetAllProgress()
        {
            _inventoryService.ResetInventory();
            _levelService.ResetPlayerProgress();
            _billingService.SetCreditsCount(0);
        }
        
        public void SetSwipeControl(bool value)
        {
            SettingsModel settingsModel = RequireSettingsModel();
            settingsModel.IsSwipe = value;
            _settingsRepository.Set(settingsModel);
        }
        
        public bool GetSwipeControl()
        {
            return RequireSettingsModel().IsSwipe;
        }
    }
}