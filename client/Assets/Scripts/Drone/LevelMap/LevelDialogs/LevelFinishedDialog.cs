using System.Linq;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Buttons;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.LevelMap.UI.DescriptionLevelDialog;
using Drone.Levels.Descriptor;
using Drone.Levels.Model;
using Drone.Levels.Service;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFinishedDialog : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFinishedDialog@embeded";

        private string _levelId;

        private LevelViewModel _levelViewModel;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;
        [UIComponentBinding("Star")]
        private ToggleButton _star;
        [UIObjectBinding("Next")]
        private GameObject _next;

        private LevelDescriptor _nextDescriptor;

        [UICreated]
        public void Init()
        {
            _star.Interactable = false;
            _star.IsOn = true;
            _levelId = _levelService.SelectedLevelId;
            _levelViewModel = _levelService.GetLevels().Find(x => x.LevelDescriptor.Id.Equals(_levelId));
            _nextDescriptor = _levelService.GetNextLevelDescriptor(_levelId);
            if (_nextDescriptor == null) {
                _next.SetActive(false);
            }
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _dialogManager.Require().Show<DescriptionLevelDialog>(_levelService.GetLevels().First(x => x.LevelDescriptor.Id == _levelId));
        }

        [UIOnClick("NextButton")]
        private void NextLevelButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _dialogManager.Require().Show<DescriptionLevelDialog>(_levelService.GetLevels().First(x => x.LevelDescriptor.Id == _nextDescriptor.Id));
        }

        [UIOnClick("MainMenuButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }
    }
}