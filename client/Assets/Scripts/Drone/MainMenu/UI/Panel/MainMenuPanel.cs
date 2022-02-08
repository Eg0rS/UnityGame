using Adept.Logger;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Dialog.Service;
using AgkUI.Element.Text;
using Drone.Billing.Event;
using Drone.Billing.Service;
using Drone.Core;
using Drone.LevelMap.UI;
using Drone.Settings.UI;
using IoC.Attribute;
using UnityEngine;

namespace Drone.MainMenu.UI.Panel
{
    [UIController(PREFAB)]
    public class MainMenuPanel : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<MainMenuPanel>();
        private const string PREFAB = "UI_Prototype/Panel/MainMenuScreen/pfMainScreenPanel@embeded";

        [Inject]
        private OverlayManager _overlayManager;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private UIService _uiService;

        [Inject]
        private DialogManager _dialogManager;

        [UIObjectBinding("LevelMapPanel")]
        private GameObject _levelMapPanel;

        [UIComponentBinding("ValueStandart")]
        private UILabel _standartValue;

        [UIComponentBinding("ValueGem")]
        private UILabel _gemValue;

        public void Init()
        {
            _overlayManager.HideLoadingOverlay(true);
            _uiService.Create<LevelsMapController>(UiModel.Create<LevelsMapController>().Container(_levelMapPanel)).Done();
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
            _standartValue.text = _billingService.GetCreditsCount().ToString();
            _gemValue.text = _billingService.GetCryptoCount().ToString();
        }

        private void OnResourceUpdated(BillingEvent resourceEvent)
        {
            UpdateCredits();
        }

        [UIOnClick("SettingsButton")]
        private void OnSettingsPanel()
        {
            _dialogManager.Show<GameSettingsDialog>();
            _logger.Debug("Click on settings");
        }
    }
}