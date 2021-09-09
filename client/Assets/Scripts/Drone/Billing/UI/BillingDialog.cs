using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using Drone.Billing.Descriptor;
using Drone.Billing.Event;
using Drone.Billing.IoC;
using Drone.Billing.Service;
using Drone.Core.UI.Dialog;
using UnityEngine;

namespace Drone.Billing.UI
{
    [UIController("UI/Dialog/pfCreditShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class BillingDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<BillingDialog>();

        [Inject]
        private BillingService _billingService;

        [Inject]
        private UIService _uiService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private BillingDescriptorRegistry _billingDescriptorRegistry;

        [UIComponentBinding("CountChips")]
        private UILabel _countChips;

        [UIComponentBinding("CountCrypto")]
        private UILabel _countCrypto;

        private readonly List<BillingItemController> _billingItemControllers = new List<BillingItemController>();

        [UICreated]
        public void Init()
        {
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            UpdateCredits();
            CreateBillingItems();
        }

        private void CreateBillingItems()
        {
            GameObject itemContainer = GameObject.Find("ScrollBillingContainer");
            foreach (BillingDescriptor item in _billingDescriptorRegistry.BillingDescriptors) {
                _uiService.Create<BillingItemController>(UiModel.Create<BillingItemController>(item).Container(itemContainer))
                          .Then(controller => { _billingItemControllers.Add(controller); })
                          .Done();
            }
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

        [UIOnClick("CloseButton")]
        private void CloseButton()
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            _dialogManager.Require().Hide(gameObject);
            _billingService.RemoveListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
        }

        [UIOnClick("DroneStoreButton")]
        private void DroneStoreButton()
        {
            CloseDialog();
            _billingService.ShowDronStoreDialog();
        }
        
        [UIOnClick("StoreCryptoButton")]
        private void OnCryptoPanel()
        {
            _billingService.SetCryptoCount(_billingService.GetCryptoCount() + 5);
        }
    }
}