using AgkCommons.Event;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using DeliveryRush.Location.Service;
using DeliveryRush.World.Event;
using IoC.Attribute;
using UnityEngine;

namespace DeliveryRush.Location.UI.Screen
{
    [UIController("UI/Screen/pfLocationScreen@embeded")]
    public class LocationScreen : MonoBehaviour
    {
        [Inject]
        private GameOverlayManager _gameOverlayManager;

        [UICreated]
        private void Init()
        {
            gameObject.GetComponent<GameEventDispatcher>().AddListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnDestroy()
        {
            if (this.IsDestroyed()) {
                return;
            }
            gameObject.GetComponent<GameEventDispatcher>().RemoveListener<WorldEvent>(WorldEvent.WORLD_CREATED, OnWorldCreated);
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
            _gameOverlayManager.LoadGameOverlay(worldEvent.DronStats);
        }
    }
}