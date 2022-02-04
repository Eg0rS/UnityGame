using System.Linq;
using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.LevelMap.UI.DescriptionLevelDialog;
using Drone.Levels.Service;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class FailLevelDialog : GameEventDispatcher
    {
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedCompactDialog@embeded";

        private string _levelId;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;

        [UICreated]
        public void Init()
        {
            _levelId = _levelService.SelectedLevelId;
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _dialogManager.Require().Show<DescriptionLevelDialog>(_levelService.GetLevels().First(x => x.LevelDescriptor.Id == _levelId));
        }

        [UIOnClick("MainMenuButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }
    }
}