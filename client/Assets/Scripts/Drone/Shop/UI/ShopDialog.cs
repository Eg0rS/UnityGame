using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using Drone.Billing.Event;
using Drone.Billing.Service;
using Drone.Billing.UI;
using Drone.Core.UI.Dialog;
using Drone.Inventory.Service;
using Drone.Shop.Descriptor;
using Drone.Shop.Event;
using Drone.Shop.Service;
using UnityEngine;

namespace Drone.Shop.UI
{
    [UIController("UI/Dialog/pfShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class ShopDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ShopDialog>();

        [Inject]
        private UIService _uiService;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        [Inject]
        private ShopDescriptor _shopDescriptor;

        [Inject]
        private ShopService _shopService;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private IGestureService _gestureService;

        private EndlessScrollView _endlessScroll;

        [UIComponentBinding("CountChips")]
        private UILabel _countChips;
        
        [UIComponentBinding("CountCrypto")]
        private UILabel _countCrypto;

        [UICreated]
        public void Init()
        {
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            _shopService.AddListener<ShopEvent>(ShopEvent.CLOSE_DIALOG, OnCloseUpdated);
            UpdateCredits();
            CreateShopItem();
            _countCrypto.text = _billingService.GetCryptoCount().ToString();
        }

        private void OnCloseUpdated(ShopEvent dialogEvent)
        {
            CloseDialog();
        }

        private void OnResourceUpdated(BillingEvent resourceEvent)
        {
            UpdateCredits();
        }

        private void UpdateCredits()
        {
            _countChips.text = _billingService.GetCreditsCount().ToString();
        }

        private void CreateShopItem()
        {
            List<ShopItemDescriptor> shopItemDescriptors = _shopDescriptor.ShopItemDescriptors;
            GameObject itemContainer = GameObject.Find("ScrollContainer");
            _endlessScroll = itemContainer.GetComponent<EndlessScrollView>();
            foreach (ShopItemDescriptor itemDescriptor in shopItemDescriptors) {
                bool isHasItem = _inventoryService.Inventory.HasItem(itemDescriptor.Id);
                _uiService.Create<ShopItemPanel>(UiModel.Create<ShopItemPanel>(itemDescriptor, isHasItem).Container(itemContainer))
                          .Then(controller => { _endlessScroll.ScrollPanelList.Add(controller.gameObject); })
                          .Then(() => { _endlessScroll.Init(); })
                          .Done();
            }
        }

        [UIOnClick("LeftButton")]
        private void OnLeftClick()
        {
            _logger.Debug("clickLeft");
            _endlessScroll.MoveRight();
        }

        [UIOnClick("RightButton")]
        private void OnRightClick()
        {
            _logger.Debug("clickRight");
            _endlessScroll.MoveLeft();
        }
        
        [UIOnClick("CloseButton")]
        private void CloseButton()
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            _dialogManager.Require().Hide(gameObject);
            _shopService.RemoveListener<ShopEvent>(ShopEvent.CLOSE_DIALOG, OnCloseUpdated);
            _billingService.RemoveListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
        }

        [UIOnClick("StoreChipsButton")]
        private void OnCreditsPanel()
        {
            CloseDialog();
            _logger.Debug("Click on credits");
            _dialogManager.Require().Show<BillingDialog>();
        }
    }
}