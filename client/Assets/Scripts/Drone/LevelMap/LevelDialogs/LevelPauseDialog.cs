using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.Levels.Service;
using Drone.Location.World;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using UnityEngine;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelPauseDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/Pause/pfPause@embeded";

        [Inject]
        private DialogManager _dialogManager;

        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private DroneWorld _droneWorld;
        [Inject]
        private LevelService _levelService;

        [UIOnClick("MainMenuButton")]
        private void MainMenuClick()
        {
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        [UIOnClick("RestartButton")]
        private void RestartClick()
        {
            _dialogManager.Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelService.SelectedLevelId);
        }
        
        
        [UIOnClick("ResumeButton")]
        private void ResumeClick()
        {
            _dialogManager.Hide(this);
            _droneWorld.Resume();
        }
    }
}