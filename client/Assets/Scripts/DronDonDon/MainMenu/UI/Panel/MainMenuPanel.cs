using System;
using System.Reflection;
using DronDonDon.MainMenu.UI.Settings.UI;
using Adept.Logger;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using DronDonDon.Core;
using DronDonDon.Dron.Model;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using LocationService = DronDonDon.Location.Service.LocationService;

namespace DronDonDon.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
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
            Vector2 swipeStartPoint;
            Vector2 swipeEndPoint;
            Vector2 swipeVector;
            String movement = "";

            swipeStartPoint = swipe.Position;
            swipeEndPoint = (Vector2) typeof(Swipe).GetField("_endPoint", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(swipe);
            
            swipeVector = new Vector2(swipeEndPoint.x - swipeStartPoint.x, swipeEndPoint.y - swipeStartPoint.y);
            swipeVector.Normalize();
            
            // Вверх 
            if ( swipeVector.y > 0 && swipeVector.x > -0.5f && swipeVector.x < 0.5f ) {
                movement = SwipeType.UP.ToString();
            }
            // Вниз
            else if ( swipeVector.y < 0 && swipeVector.x > -0.5f && swipeVector.x < 0.5f ) {
                movement = SwipeType.DOWN.ToString();
            }
            // Влево
            else if ( swipeVector.x < 0 && swipeVector.y > -0.5f && swipeVector.y < 0.5f ) {
                movement = SwipeType.LEFT.ToString();
            }
            // Вправо
            else if ( swipeVector.x > 0 && swipeVector.y > -0.5f && swipeVector.y < 0.5f ) {
                movement = SwipeType.RIGHT.ToString();
            }
            // Вверх влево
            else if ( swipeVector.y > 0 && swipeVector.x < 0 ) {
                movement = SwipeType.UP_LEFT.ToString();
            }
            // Вверх вправо
            else if ( swipeVector.y > 0 && swipeVector.x > 0 ) {
                movement = SwipeType.UP_RIGHT.ToString();
            }
            // Вниз влево
            else if ( swipeVector.y < 0 && swipeVector.x < 0 ) {
                movement = SwipeType.DOWN_LEFT.ToString();
                
            // Вниз вправо
            } else if ( swipeVector.y < 0 && swipeVector.x > 0 )
            {
                movement = SwipeType.DOWN_RIGHT.ToString();
            }
            _logger.Debug("Тип свайпа: " + movement);
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