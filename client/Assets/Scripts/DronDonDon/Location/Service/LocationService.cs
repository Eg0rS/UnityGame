using System.Reflection;
using AgkCommons.CodeStyle;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Location.World.Dron;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Assets.Scripts.DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService 
    {
        private const string LOCATION_PREFAB = "World/Location/pfLocation@embeded";
        
        [Inject] 
        private ScreenManager _screenManager;   
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
       
        public void StartGame()
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            CreatedWorld();
        }
        private void CreatedWorld()
        {
            _locationBuilderManager.CreateDefault()
                                   .Prefab(LOCATION_PREFAB)
                                   .Build()
                                   .Then(() => {
                                       _overlayManager.Require().HideLoadingOverlay(true);
                                   })
                                   .Done();
        }
        
    }
}