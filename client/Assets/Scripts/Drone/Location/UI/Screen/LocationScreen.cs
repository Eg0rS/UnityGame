using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using Drone.Location.Service;
using GameKit.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using LocationService = Drone.Location.Service.LocationService;

namespace Drone.Location.UI.Screen
{
    [UIController("UI/Screen/pfLocationScreen@embeded")]
    public class LocationScreen : MonoBehaviour
    {
        [Inject]
        private GameOverlayManager _gameOverlayManager;

        [Inject]
        private IoCProvider<LocationService> _locationService;

        [UICreated]
        private void Init()
        {
            _gameOverlayManager.LoadGameOverlay();
            //_locationService.Require().AddListener<WorldObjectEvent>(WorldObjectEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnDestroy()
        {
            if (this.IsDestroyed()) {
                return;
            }
            //_locationService.Require().RemoveListener<WorldObjectEvent>(WorldObjectEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnWorldCreated(WorldObjectEvent worldObjectEvent)
        {
            _gameOverlayManager.LoadGameOverlay();
        }
    }
}