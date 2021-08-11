using AgkCommons.CodeStyle;
using AgkUI.Core.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Location.World.Dron.Service;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService
    {
        [Inject]
        private ScreenManager _screenManager;  
        
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        
        [Inject]
        private IoCProvider<GameService> _gameService;

        [Inject] private UIService _uiService;

        [Inject] 
        private DronService _dronService;
        
        public void StartGame(LevelDescriptor level, string dronId)
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld(level, dronId);
        }
        private void CreatedWorld(LevelDescriptor level, string dronId)
        {
            string levelPrefabName = level.Prefab;
            _locationBuilderManager.CreateDefault()
                                   .Prefab(levelPrefabName)
                                   .Build()
                                   .Then(() =>
                                   {
                                       string path  = _dronService.GetDronById(dronId).DronDescriptor.Prefab;
                                       GameObject.Find("dronx").GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>(path);
                                       _gameService.Require().StartGame(level,dronId);
                                   })
                                   .Done();
        }
    }
}