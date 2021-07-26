using System.Reflection;
using DronDonDon.MainMenu.UI.Settings.UI;
using Adept.Logger;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using DronDonDon.Core;
using DronDonDon.Dron.Controller;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using LocationService = DronDonDon.Location.Service.LocationService;

namespace DronDonDon.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private DronController dc = new DronController();

        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI/Panel/pfMainScreenPanel@embeded";
        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;       
        [Inject]
        private LocationService _locationService;

        [Inject] 
        private IoCProvider<DialogManager> _dialogManager;
        
        [Inject]
        private IGestureService _gestureService;

        [UICreated]
        public void Init()
        {
            _overlayManager.Require().HideLoadingOverlay(true);
            _logger.Debug("MainMenuPanel start init");
            
            _gestureService.AddSwipeHandler(OnSwiped, false);
        }

        
        private void OnSwiped(Swipe swipe)
        {
            dc.ShiftVirtualPosition(SwipeToSector(swipe));
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

        [UIOnClick("MiddlePanel")]
        private void OnMiddleClick()
        {
            _logger.Debug("OnMiddleClick");
        }
        
        [UIOnClick("StartGameButton")]
        private void OnStartGame()
        {
            _locationService.StartGame();
        }

        [UIOnClick("DronShop")]
        private void OnDroneStore()
        { 
            _logger.Debug("Click on store");
        }
        [UIOnClick("SettingsButton")]
        private void OnSettingsPanel()
        {
            _dialogManager.Require().Show<GameSettingsDialog>();
            _logger.Debug("Click on settings");
            
        }
        [UIOnClick("StoreChipsButton")]
        private void OnCreditsPanel()
        {
            _logger.Debug("Click on credits");
        }
    }
}