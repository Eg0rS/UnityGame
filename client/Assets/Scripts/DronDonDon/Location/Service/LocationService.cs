using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Settings.UI;
using DronDonDon.World;
using DronDonDon.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService: GameEventDispatcher
    {
        [Inject]
        private ScreenManager _screenManager;  
        
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject] 
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject] private UIService _uiService;
        
        public void StartGame(string levelPrefabName)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName);
            
            //
            // _uiService.Create<DronStatsDialog>(UiModel
            //         .Create<DronStatsDialog>()
            //         .Container(levelContainer))
            //     .Done();
        }
        private void CreatedWorld(string levelPrefabName)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                       GameObject levelContainer = GameObject.Find($"Overlay");
                                       _uiService.Create<DronStatsDialog>(UiModel
                                               .Create<DronStatsDialog>()
                                               .Container(levelContainer))
                                           .Done();
                                       
                                   })
                                   //
                                   .Done();
        }
    }
}