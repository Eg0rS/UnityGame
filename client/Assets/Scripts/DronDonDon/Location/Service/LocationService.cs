using AgkCommons.CodeStyle;
using AgkCommons.Event;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.LevelDialogs;
using DronDonDon.Game.Levels.Descriptor;
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

        [Inject] 
        private GameService _gameService;
        
        [Inject] private UIService _uiService;

        [Inject] 
        private DronService _dronService;

        private LevelDescriptor _levelDescriptor;
        
        public void StartGame(LevelDescriptor levelDescriptor, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(levelDescriptor, dronId);
        }
        private void CreatedWorld(LevelDescriptor levelDescriptor, string dronId)
        {
            string levelPrefabName = levelDescriptor.Prefab;
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() =>
                                   {
                                       string path  = _dronService.GetDronById(dronId).DronDescriptor.Prefab;
                                       GameObject.Find("dronx").GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>(path); 
                                       _gameService.StartGame(levelDescriptor,dronId);
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }
    }
}