using AgkCommons.Event;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using Drone.Location.Event;
using Drone.Location.Service;
using IoC.Attribute;
using UnityEngine;

namespace Drone.Location.UI.Screen
{
    [UIController("UI_Prototype/Screen/pfLocationScreen@embeded")]
    public class LocationScreen : MonoBehaviour
    {
        [Inject]
        private GameOverlayManager _gameOverlayManager;

        [UICreated]
        private void Init()
        {
            gameObject.GetComponent<GameEventDispatcher>().AddListener<WorldEvent>(WorldEvent.CREATED, OnWorldCreated);
        }

        private void OnDestroy()
        {
            if (this.IsDestroyed()) {
                return;
            }
            gameObject.GetComponent<GameEventDispatcher>().RemoveListener<WorldEvent>(WorldEvent.CREATED, OnWorldCreated);
        }

        private void OnWorldCreated(WorldEvent worldEvent)
        {
            _gameOverlayManager.LoadGameOverlay();
        }
    }
}