using DronDonDon.Settings.UI;
using DronDonDon.Billing.UI;
using DronDonDon.Billing.Event;
using DronDonDon.Billing.Service;
using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using DronDonDon.Core;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;
using LocationService = Assets.Scripts.DronDonDon.Location.Service.LocationService;

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
        private BillingService _billingService;
        
        [UIComponentBinding("CountChips")]
        private UILabel _countChips;

        [UICreated]
        public void Init()
        {
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            _overlayManager.Require().HideLoadingOverlay(true);
            UpdateCredits();
            _logger.Debug("MainMenuPanel start init");
        }
        
        private void UpdateCredits()
        {
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }
        
        private void OnResourceUpdated(BillingEvent resourceEvent)
        {
            UpdateCredits();
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
            _dialogManager.Require().Show<CreditShopDialog>();
        }
    }
}