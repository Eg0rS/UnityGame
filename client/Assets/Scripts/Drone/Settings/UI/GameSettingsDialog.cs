using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using Drone.Core.UI.Dialog;
using Drone.Location.Service;
using Drone.Settings.Service;
using UnityEngine;

namespace Drone.Settings.UI
{
    [UIController("UI/Dialog/pfSettingsDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class GameSettingsDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<GameSettingsDialog>();

        [UIComponentBinding("SoundToggleButton")]
        private ToggleButton _toggleSoundButton;

        [UIComponentBinding("MusicToggleButton")]
        private ToggleButton _toggleMusicButton;
        
        [UIComponentBinding("SwipeToggleButton")]
        private ToggleButton _swipeButton;

        [Inject]
        private SettingsService _settingsService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        private void Start()
        {
            _toggleMusicButton.IsOn = _settingsService.GetMusicMute();
            _toggleSoundButton.IsOn = _settingsService.GetSoundMute();
            _swipeButton.IsOn = _settingsService.GetSwipeControl();
        }

        [UIOnClick("CloseButton")]
        private void CloseButton()
        {
            _dialogManager.Require().Hide(gameObject);
        }

        [UIOnClick("SoundToggleButton")]
        private void OnSoundButton()
        {
            _logger.Debug("MuteSound");
            _settingsService.SetSoundMute(_toggleSoundButton.IsOn);
        }
        
        [UIOnClick("SwipeToggleButton")]
        private void OnSwipeButton()
        {
            _settingsService.SetSwipeControl(_swipeButton.IsOn);
        }

        [UIOnClick("MusicToggleButton")]
        private void OnMusicButton()
        {
            _logger.Debug("MuteMusic");
            _settingsService.SetMusicMute(_toggleMusicButton.IsOn);
        }

        [UIOnClick("ResetButton")]
        private void OnResetButton()
        {
            _logger.Debug("Reset");
            _settingsService.ResetAllProgress();
        }
    }
}