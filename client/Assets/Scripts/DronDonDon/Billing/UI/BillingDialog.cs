using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using DronDonDon.Core.UI.Dialog;
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
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }
        
        [UIOnClick("CloseButton")]
        private void CloseButton()
        {
            _dialogManager.Require()
                .Hide(gameObject);
        }
        
        [UIOnClick("BlueButton")]
        private void On32ChipsButton()
        {
            _logger.Debug("+10Chips");
            _billingService.SetCreditsCount(_billingService.GetCreditsCount()+10);
            _countChips.text = _billingService.GetCreditsCount().ToString();
            _logger.Debug(_billingService.GetCreditsCount().ToString());
        }
        [UIOnClick("GreenButton")]
        private void On64ChipsButton()
        {
            _logger.Debug("+50Chips");
            _billingService.SetCreditsCount(_billingService.GetCreditsCount()+50);
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }
        [UIOnClick("GreenButton")]
        private void On128ChipsButton()
        {
            _logger.Debug("+100");
            _billingService.SetCreditsCount(_billingService.GetCreditsCount()+100);
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }
        [UIOnClick("RedButton")]
        private void On256ChipsButton()
        {
            _logger.Debug("+500");
            _billingService.SetCreditsCount(_billingService.GetCreditsCount()+500);
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }
        
        [UIOnClick("DroneShopButton")]
        private void OnDroneShopButton()
        {
            _dialogManager.Require()
                .Hide(gameObject);
            
            //_dialogManager.Require().Show<DroneShopDialog>();
        }
    }
}