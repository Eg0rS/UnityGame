using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
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
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName);
            
            OnLevelFinished();
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

        public void OnLevelFinished()
        {
            /* Показываем диалог о завершении уровня.
               Передаём в него ViewModel, откуда берём исходные и конечные данные            
            */
            _dialogManager.Require().Show<LevelFinishedDialog>(_levelService.v);
        }

        public void OnLevelFailed()
        {
            /* Показываем диалог о проигрыше.
               Передаём в него причину поражения: закончилась энергия или прочность            
            */
        }
    }
}