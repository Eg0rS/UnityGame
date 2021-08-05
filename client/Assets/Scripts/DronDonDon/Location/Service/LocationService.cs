using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using IoC.Attribute;
using IoC.Util;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService 
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<LocationService>(); 
        
        [Inject]
        private ScreenManager _screenManager;
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        [Inject]
        private LevelService _levelService;

        public void StartGame(string levelPrefabName)
        {
            /*_overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName);*/
            
            
            _dialogManager.Require().Show<LevelFinishedDialog>();
        }
        
        private void CreatedWorld(string levelPrefabName)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }

        private void OnLevelFinished()
        {
            
        }

        private void OnLevelFailed()
        {
            
        }
    }
}