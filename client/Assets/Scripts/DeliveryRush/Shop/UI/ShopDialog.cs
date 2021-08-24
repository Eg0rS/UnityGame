﻿using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using DeliveryRush.Billing.Event;
using DeliveryRush.Billing.Service;
using DeliveryRush.Billing.UI;
using DeliveryRush.Core.UI.Dialog;
using DeliveryRush.Inventory.Service;
using DeliveryRush.Shop.Descriptor;
using DeliveryRush.Shop.Event;
using DeliveryRush.Shop.Service;
using UnityEngine;

namespace DeliveryRush.Shop.UI
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

        private ListPositionCtrl _listPositionCtrl;
        private readonly List<ShopItemPanel> _shopItemPanels = new List<ShopItemPanel>();

        [UIComponentBinding("CountChips")]
        private UILabel _countChips;

        [UICreated]
        public void Init()
        {
            _billingService.AddListener<BillingEvent>(BillingEvent.UPDATED, OnResourceUpdated);
            _shopService.AddListener<ShopEvent>(ShopEvent.CLOSE_DIALOG, OnCloseUpdated);
            UpdateCredits();
            CreateShopItem();
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
            foreach (ShopItemDescriptor itemDescriptor in shopItemDescriptors) {
                bool isHasItem = _inventoryService.Inventory.HasItem(itemDescriptor.Id);
                _uiService.Create<ShopItemPanel>(UiModel.Create<ShopItemPanel>(itemDescriptor, isHasItem).Container(itemContainer))
                          .Then(controller => { _shopItemPanels.Add(controller); })
                          .Done();
            }
            _uiService.Create<ScrollController>(UiModel.Create<ScrollController>(_shopItemPanels).Container(itemContainer))
                      .Then(controller => { _listPositionCtrl = controller.Control; })
                      .Done();
        }

        [UIOnClick("LeftButton")]
        private void OnLeftClick()
        {
            _logger.Debug("clickLeft");
            MoveLeft();
        }

        [UIOnClick("RightButton")]
        private void OnRightClick()
        {
            _logger.Debug("clickRight");
            MoveRight();
        }

        private void MoveLeft()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(1);
        }

        private void MoveRight()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(-1);
            ;
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
            _dialogManager.Require().Show<CreditShopDialog>();
        }
    }
}