using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Service;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Location.World.Dron.Service;
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

        [Inject] 
        private DronService _dronService;
        
        public void StartGame(string levelPrefabName, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelPrefabName, dronId);
        }
        private void CreatedWorld(string levelPrefabName, string dronId)
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() =>
                                   {
                                       string path  = _dronService.GetDronById(dronId).DronDescriptor.Prefab;
                                       GameObject.Find("dronx").GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>(path);
                                       
                                       GameObject levelContainer = GameObject.Find($"Overlay");
                                       _uiService.Create<DronStatsDialog>(UiModel
                                               .Create<DronStatsDialog>()
                                               .Container(levelContainer))
                                           .Done();
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                       
                                   })
                                   //
                                   .Done();
        }
    }
}