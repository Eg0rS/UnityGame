using AgkCommons.CodeStyle;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Game.Levels.Descriptor;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI;
using DronDonDon.Location.UI.Screen;
using DronDonDon.Location.World.Dron.Descriptor;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService : GameEventDispatcher
    {
        [Inject] private ScreenManager _screenManager;

        [Inject] private LocationBuilderManager _locationBuilderManager;

        [Inject] private IoCProvider<OverlayManager> _overlayManager;

        [Inject] private IoCProvider<WorldService> _gameService;
        
        [Inject] private ResourceService _resourceService;


        
        public void StartGame(string levelPrefabName)
        {
            
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
           // CreatedWorld(levelPrefabName, dronDescriptor);
        }

        private void CreatedWorld(string levelPrefabName, DronDescriptor dronDescriptor)
        {
            _locationBuilderManager.CreateDefault()
                .Prefab(levelPrefabName)
                .Build()
                .Then(() =>
                {
                    _gameService.Require().StartGame(dronDescriptor);
                    
                });
        }
    }
}