using System;
using System.Reflection;
using Adept.Logger;
using AgkCommons.CodeStyle;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Binding.Attributes;
using AgkUI.Screens.Service;
using DronDonDon.Core;
using DronDonDon.Dron.Controller;
using DronDonDon.Location.Service.Builder;
using DronDonDon.Location.UI.Screen;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace DronDonDon.Location.Service
{
    [Injectable]
    public class LocationService 
    {
        private const string LOCATION_PREFAB = "World/Location/pfLocation@embeded";
        
        private DronController _dc = new DronController();

        [Inject]
        private ScreenManager _screenManager;   
        [Inject]
        private LocationBuilderManager _locationBuilderManager;
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;
        [Inject]
        private IGestureService _gestureService;

        public void StartGame()
        {
            _overlayManager.Require().ShowPreloader();
            _screenManager.LoadScreen<LocationScreen>();
            _gestureService.AddSwipeHandler(OnSwiped,false);
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

        private void OnSwiped(Swipe swipe)
        {
            _dc.MoveDron(SwipeToSector(swipe));
        }
        
        private int SwipeToSector(Swipe swipe)
        {
            Vector2 swipeEndPoint;
            Vector2 swipeVector;
            int angle;
            int result;

            swipeEndPoint = (Vector2) typeof(Swipe).GetField("_endPoint", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(swipe);
            swipeVector = swipeEndPoint - swipe.Position;
            angle = (int) Vector2.Angle(Vector2.up, swipeVector.normalized);
            
            result = Vector2.Angle(Vector2.right, swipeVector.normalized) > 90 ? 360 - angle : angle;
            return (int) Mathf.Round(result / 45f) % 8;
        }
    }
}