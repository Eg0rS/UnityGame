using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.Location.Service.Game;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelPauseDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelPauseDialog>();
        private const string PREFAB_NAME = "UI/Pause/pfLevelPauseDialog@embeded";

        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [UICreated]
        public void Init()
        {
            Time.timeScale = 0;
        }

        [UIOnClick("ButtonExit")]
        private void ExitClick()
        {
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        [UIOnClick("ButtonResume")]
        private void ResumeClick()
        {
            _dialogManager.Hide(this);
            Time.timeScale = 1;
        }
    }
}