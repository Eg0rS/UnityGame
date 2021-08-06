using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using AgkUI.Screens.Service;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.MainMenu.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Game.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelFailedCompactDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelFailedCompactDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelFailedCompactDialog@embeded";

        private string _failReason = "";
        // "Закончилась энергия"
        // "Дрон разбился"
        
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject]
        private ScreenManager _screenManager;
        
        [UIComponentBinding("FailReasonTitle")]
        private UILabel _failReasonLabel;
        
        [UICreated]
        public void Init(string failReason)
        {
            _logger.Debug("[LevelFailedCompactDialog] Init()...");

            _failReason = failReason;
            SetDialogLabels();
        }

        [UIOnClick("RestartButton")]
        private void RestartButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
            // _dialogManager.Require().Show<DescriptionLevelDialog>();
        }

        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(this);
            _screenManager.LoadScreen<MainMenuScreen>();
        }

        private void SetDialogLabels()
        {
            _failReasonLabel.text = _failReason;
        }
    }
}