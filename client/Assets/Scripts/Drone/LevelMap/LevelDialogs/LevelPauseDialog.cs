using System.Linq;
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
using IoC.Util;
using UnityEngine;
using LocationService = Drone.Location.Service.LocationService;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelPauseDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/Pause/pfPause@embeded";

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private DroneWorld _droneWorld;
        [Inject]
        private LevelService _levelService;

        [Inject]
        private LocationService _locationService;

        [UIOnClick("MainMenuButton")]
        private void MainMenuClick()
        {
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        [UIOnClick("RestartButton")]
        private void RestartClick()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _locationService.SwitchLocation(_levelService.GetLevels()
                                                         .First(x => x.LevelDescriptor.Id == _levelService.SelectedLevelId)
                                                         .LevelDescriptor);
        }

        [UIOnClick("ResumeButton")]
        private void ResumeClick()
        {
            _dialogManager.Require().Hide(this);
            _droneWorld.Resume();
        }
    }
}