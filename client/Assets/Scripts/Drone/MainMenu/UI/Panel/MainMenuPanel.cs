using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using Drone.Billing.Event;
using Drone.Billing.Service;
using Drone.Billing.UI;
using Drone.Core;
using Drone.LevelMap.Levels.UI;
using Drone.Settings.UI;
using Drone.Shop.UI;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI/Panel/pfMainScreenPanel@embeded";

        [Inject]
        private IoCProvider<OverlayManager> _overlayManager;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private UIService _uiService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [UIObjectBinding("MiddlePanel")]
        private GameObject _middlePanel;

        [UIComponentBinding("CountChips")]
        private UILabel _countChips;
        
        [UIComponentBinding("CountCrypto")]
        private UILabel _countCrypto;
        
        public void Init()
        {
            _overlayManager.Require().HideLoadingOverlay(true);
            _uiService.Create<ProgressMapController>(UiModel.Create<ProgressMapController>().Container(_middlePanel)).Done();
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            UpdateCredits();
            _logger.Debug("MainMenuPanel start init");
        }

        private void OnDestroy()
        {
            _billingService.RemoveListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
        }

        private void UpdateCredits()
        {
            _countChips.text = _billingService.GetCreditsCount().ToString();
            _countCrypto.text = _billingService.GetCryptoCount().ToString();
        }

        private void OnResourceUpdated(BillingEvent resourceEvent)
        {
            UpdateCredits();
        }

        [UIOnClick("DronShop")]
        private void OnDroneStore()
        {
            _dialogManager.Require().Show<ShopDialog>();
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
            _dialogManager.Require().Show<CreditShopDialog>();
        }
        
        // todo удалить
        [UIOnClick("StoreCryptoButton")]
        private void OnCryptoPanel()
        {
            _billingService.SetCryptoCount(_billingService.GetCryptoCount() + 5);
        }
    }
}