using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using Drone.Billing.Event;
using Drone.Billing.Service;
using Drone.Billing.UI;
using Drone.Core;
using Drone.LevelMap.UI;
using Drone.Settings.UI;
using Drone.Shop.UI;
using IoC.Attribute;
using TMPro;
using UnityEngine;

namespace Drone.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI/MainMenu/Panel/pfMainScreenPanel@embeded";

        [Inject]
        private OverlayManager _overlayManager;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private UIService _uiService;

        [Inject]
        private DialogManager _dialogManager;

        [UIObjectBinding("MiddlePanel")]
        private GameObject _middlePanel;

        [UIComponentBinding("ChipsValue")]
        private TextMeshProUGUI _countChips;

        [UIComponentBinding("CryptValue")]
        private TextMeshProUGUI _countCrypto;

        public void Init()
        {
            _overlayManager.HideLoadingOverlay(true);
            _uiService.Create<LevelsMapController>(UiModel.Create<LevelsMapController>().Container(_middlePanel)).Done();
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

        [UIOnClick("ShopButton")]
        private void OnStore()
        {
            _dialogManager.Show<ShopDialog>();
            _logger.Debug("Click on store");
        }

        [UIOnClick("ButtonSetting")]
        private void OnSettingsPanel()
        {
            _dialogManager.Show<GameSettingsDialog>();
            _logger.Debug("Click on settings");
        }

        //[UIOnClick("StatusChips")]
        private void OnCreditsPanel()
        {
            _dialogManager.Show<BillingDialog>();
        }
    }
}