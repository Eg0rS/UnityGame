using Adept.Logger;
using AgkCommons.Event;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using Drone.Core.UI.Dialog;
using Drone.LevelMap.Levels.Service;
using Drone.Location.Service;
using Drone.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;

namespace Drone.LevelMap.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFailedCompactDialog : GameEventDispatcher
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFailedCompactDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedCompactDialog@embeded";

        private string _levelId;
        private FailedReasons _failReason;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ScreenManager _screenManager;

        [Inject]
        private LevelService _levelService;

        [UIComponentBinding("FailReasonTitle")]
        private UILabel _failReasonLabel;

        [UICreated]
        public void Init(FailedReasons failReason)
        {
            _logger.Debug("[LevelFailedCompactDialog] Init()...");
            _levelId = _levelService.CurrentLevelId;

            _failReason = failReason;
            SetDialogLabels();
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            _levelService.ShowStartLevelDialog(_levelId);
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void SetDialogLabels()
        {
            switch (_failReason) {
                case FailedReasons.Crashed:
                    _failReasonLabel.text = "Дрон разбился";
                    break;
                case FailedReasons.EnergyFalled:
                    _failReasonLabel.text = "Закончилась энергия";
                    break;
            }
        }
    }
}