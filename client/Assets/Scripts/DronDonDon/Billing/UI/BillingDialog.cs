using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using DronDonDon.Core.UI.Dialog;
using DronDonDon.Billing.Event;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkUI.Element.Text;
using DronDonDon.Billing.Service;
using UnityEngine;

namespace DronDonDon.Billing.UI
{
    [UIController("UI/Dialog/pfCreditShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class CreditShopDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreditShopDialog>();
        [UIComponentBinding("CountChips")]
        private UILabel _countChips;

        [Inject] 
        private BillingService _billingService;
        
        [Inject]
        private IoCProvider<DialogManager> _dialogManager;
        
        [UICreated]
        public void Init()
        {
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            UpdateCredits();
        }

        private void UpdateCredits()
        {
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }

        private void OnResourceUpdated(BillingEvent resourceEvent)
        {
            UpdateCredits();
        }

        [UIOnClick("CloseButton")]
        private void CloseButton()
        {
            _dialogManager.Require()
                .Hide(gameObject);
            _billingService.RemoveListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
        }

        /*[UIOnClick("BlueButton")]
        private void On32ChipsButton()
        {
            _logger.Debug("+10Chips");
            _billingService.AddCredits(10);
        }
        
        [UIOnClick("GreenButton")]
        private void On64ChipsButton()
        {
            _logger.Debug("+50Chips");
            _billingService.AddCredits(50);
        }
        
        [UIOnClick("YellowButton")]
        private void On128ChipsButton()
        {
            _logger.Debug("+100Chips");
            _billingService.AddCredits(100);
        }
        
        [UIOnClick("RedButton")]
        private void On256ChipsButton()
        {
            _logger.Debug("+500Chips");
            _billingService.AddCredits(500);
        }*/
        
    }
}