using System;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using DeliveryRush.Billing.Model;
using DeliveryRush.Billing.Service;
using DeliveryRush.Core.Filter;
using DeliveryRush.Inventory.Model;
using DeliveryRush.Inventory.Service;
using DeliveryRush.Shop.Descriptor;
using DeliveryRush.Shop.Event;
using DeliveryRush.Shop.UI;
using IoC.Attribute;
using IoC.Util;

namespace DeliveryRush.Shop.Service
{
    public class ShopService : GameEventDispatcher, IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private ShopDescriptor _shopDescriptor;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private BillingService _billingService;
        [Inject]
        private PlayerResourceModel _resourceModel;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        public void Init()
        {
            _resourceService.LoadConfiguration("Configs/shop@embeded", OnConfigLoaded);
        }

        public void RemoveListener()
        {
            Dispatch(new ShopEvent(ShopEvent.CLOSE_DIALOG));
        }

        public bool BuyDron(string itemId)
        {
            _resourceModel = _billingService.RequirePlayerResourceModel();
            ShopItemDescriptor shopItemDescriptor = _shopDescriptor.GetShopItem(itemId);
            if (shopItemDescriptor == null) {
                throw new Exception("ShopItem not found, itemId = " + itemId);
            }
            if (_resourceModel.CreditsCount >= shopItemDescriptor.Price) {
                _billingService.SetCreditsCount(_resourceModel.CreditsCount - shopItemDescriptor.Price);
                InventoryItemModel item = new InventoryItemModel(itemId, shopItemDescriptor.Type, 1);
                _inventoryService.AddInventory(item);
                return true;
            } else {
                _dialogManager.Require().ShowModal<BuyDialog>(false);
                return false;
            }
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("shop.shopItem")) {
                ShopItemDescriptor shopItemDescriptor = new ShopItemDescriptor();
                shopItemDescriptor.Configure(temp);
                _shopDescriptor.ShopItemDescriptors.Add(shopItemDescriptor);
            }
        }
    }
}