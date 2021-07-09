using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using DronDonDon.Core;
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
    
        [UICreated]
        public void Init()
        {
            _overlayManager.Require().HideLoadingOverlay(true);
            _logger.Debug("MainMenuPanel start init");
        }
        
        [UIOnClick("StartGameButton")]
        private void OnStartGame()
        {
            _locationService.StartGame();
        }
    }
}