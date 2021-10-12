using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using Drone.Core.UI.Dialog;
using Drone.Settings.Service;
using UnityEngine;

namespace Drone.Settings.UI
{
    [UIController("UI/Dialog/pfSettingsDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class GameSettingsDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<GameSettingsDialog>();

        [UIComponentBinding("Handle_Sound")]
        private SwitchButton _toggleSoundButton;

        [UIComponentBinding("Handle_Music")]
        private SwitchButton _toggleMusicButton;
        
        [Inject]
        private SettingsService _settingsService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        private void Start()
        {
            _toggleMusicButton._isSwitchOn = _settingsService.GetMusicMute();
            _toggleSoundButton._isSwitchOn = _settingsService.GetSoundMute();
            _toggleMusicButton.SwitchCheck();
            _toggleSoundButton.SwitchCheck();
        }

        [UIOnClick("Button_close")]
        private void CloseButton()
        {
            _dialogManager.Require().Hide(gameObject);
        }

        [UIOnClick("Handle_Sound")]
        private void OnSoundButton()
        {
            _logger.Debug("MuteSound");
            _settingsService.SetSoundMute(_toggleSoundButton._isSwitchOn);
        }
        
        [UIOnClick("Handle_Music")]
        private void OnMusicButton()
        {
            _logger.Debug("MuteMusic");
            _settingsService.SetMusicMute(_toggleMusicButton._isSwitchOn);
        }

        // [UIOnClick("ResetButton")]
        // private void OnResetButton()
        // {
        //     _logger.Debug("Reset");
        //     _settingsService.ResetAllProgress();
        // }
    }
}