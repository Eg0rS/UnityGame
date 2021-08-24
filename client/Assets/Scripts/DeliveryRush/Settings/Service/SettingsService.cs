using DeliveryRush.Billing.Service;
using DeliveryRush.Core.Audio.Service;
using DeliveryRush.Core.Filter;
using DeliveryRush.Game.Levels.Service;
using DeliveryRush.Inventory.Service;
using DeliveryRush.Settings.Model;
using IoC.Attribute;

namespace DeliveryRush.Settings.Service
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
    }
}