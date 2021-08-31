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
using DeliveryRush.Billing.Descriptor;
using DeliveryRush.Billing.Event;
using DeliveryRush.Billing.IoC;
using DeliveryRush.Billing.Service;
using DeliveryRush.Core.UI.Dialog;
using UnityEngine;

namespace DeliveryRush.Billing.UI
{
    [UIController("UI/Dialog/pfCreditShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class CreditShopDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<CreditShopDialog>();

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
    }
}