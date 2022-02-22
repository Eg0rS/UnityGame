using AgkCommons.Event;
using Drone.Billing.Service;
using Drone.Core.Audio.Service;
using Drone.Core.Service;
using Drone.Inventory.Service;
using Drone.Levels.Service;
using Drone.Settings.Model;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Settings.Service
{
    public class SettingsService : GameEventDispatcher, IConfigurable
    {
        [Inject]
        private SettingsRepository _settingsRepository;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private IoCProvider<LevelService> _levelService;

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
                Configure();
            }
            return _settingsRepository.Require();
        }

        public void Configure()
        {
            if (!HasSettingsModel()) {
                SettingsModel settingsModel = new SettingsModel {
                        IsMusicMute = true,
                        IsSoundMute = true,
                        Seed = UnityEngine.Random.Range(0, 10000)
                };
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

        public int GetSeed()
        {
            return RequireSettingsModel().Seed;
        }

        public void ResetAllProgress()
        {
            _inventoryService.ResetInventory();
            _levelService.Require().ResetPlayerProgress();
            _billingService.SetCreditsCount(0);
        }
    }
}