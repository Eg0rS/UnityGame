using AgkCommons.Event;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using Drone.Location.Service;
using Drone.World.Event;
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
            _locationService.Require().AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            //gameObject.GetComponent<GameEventDispatcher>().AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
                    // _gameOverlayManager.LoadGameOverlay();
        }

        private void OnDestroy()
        {
            if (this.IsDestroyed()) {
                return;
            }
            _locationService.Require().RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
            //gameObject.GetComponent<GameEventDispatcher>().RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
             _gameOverlayManager.LoadGameOverlay();
        }
    }
}