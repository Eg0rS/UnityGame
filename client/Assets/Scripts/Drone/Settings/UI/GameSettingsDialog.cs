using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.Core.UI.Dialog;
using Drone.Settings.Service;
using UnityEngine;

namespace Drone.Settings.UI
{
    [UIController("UI_Prototype/Dialog/Settings/pfSettingsDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class GameSettingsDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<GameSettingsDialog>();

        [Inject]
        private SettingsService _settingsService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [UIComponentBinding("SoundButton")]
        private ToggleButton _soundButton;

        [UIComponentBinding("MusicButton")]
        private ToggleButton _musicButton;
        [UIComponentBinding("Seed")]
        private UILabel _seed;

        [UICreated]
        private void Init()
        {
            _soundButton.IsOn = _settingsService.GetSoundMute();
            _musicButton.IsOn = _settingsService.GetMusicMute();
            _soundButton.OnClick.AddListener(OnSoundButton);
            _musicButton.OnClick.AddListener(OnMusicButton);
            _seed.text = "seed: " + _settingsService.GetSeed();
        }

        private void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
                CloseDialog();
            }
        }

        [UIOnClick("Close")]
        private void CloseDialog()
        {
            _dialogManager.Require().Hide(gameObject);
        }

        private void OnSoundButton()
        {
            _logger.Debug("MuteSound");
            _settingsService.SetSoundMute(_soundButton.IsOn);
        }

        private void OnMusicButton()
        {
            _logger.Debug("MuteMusic");
            _settingsService.SetMusicMute(_musicButton.IsOn);
        }
    }
}