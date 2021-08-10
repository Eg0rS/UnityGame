using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Service;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.MainMenu.UI.Screen;
using DronDonDon.World;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Game.LevelDialogs
{
    [UIController(PREFAB_NAME)]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class LevelPauseDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LevelPauseDialog>();
        private const string PREFAB_NAME = "UI/Dialog/pfLevelPauseDialog@embeded";

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        
        [Inject] 
        private OverlayManager _overlayManager;

        [Inject] private UIService _uiService;
        
        [Inject]
        private ScreenManager _screenManager;
        
        [UICreated]
        public void Init()
        {
            _logger.Debug("[LevelPauseDialog] Init()...");
            Time.timeScale = 0;
        }
        
        [UIOnClick("LevelMapButton")]
        private void LevelMapButtonClicked()
        {
            _dialogManager.Require().Hide(gameObject);
            
            _gameWorld.Require().RemoveGameObject("Overlay");
            _screenManager.LoadScreen<MainMenuScreen>();
        }
        
        [UIOnClick("ContinueButton")]
        private void ContinueButtonClicked()
        {
            _dialogManager.Require().Hide(gameObject);
            Time.timeScale = 1;
        }
    }
}