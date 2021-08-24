using AgkUI.Binding.Attributes;
using DeliveryRush.Core;
using IoC.Attribute;
using UnityEngine;

namespace DeliveryRush.Location.UI.Screen
{
    [UIController("UI/Screen/pfLocationScreen@embeded")]
    public class LocationScreen : MonoBehaviour
    {
        [Inject]
        private OverlayManager _overlayManager;

        [UICreated]
        private void Init()
        {
        }

        private void OnDestroy()
        {
        }

        private void OnWorldCreated()
        {
        }

        private void AddUIScreenLoader()
        {
        }
    }
}