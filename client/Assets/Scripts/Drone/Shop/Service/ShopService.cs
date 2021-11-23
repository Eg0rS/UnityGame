using System;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using Drone.Billing.Model;
using Drone.Billing.Service;
using Drone.Core.Filter;
using Drone.Inventory.Model;
using Drone.Inventory.Service;
using Drone.Shop.Descriptor;
using Drone.Shop.Event;
using Drone.Shop.UI;
using IoC.Attribute;
using IoC.Util;
using JetBrains.Annotations;

namespace Drone.Shop.Service
{
    public class ShopService : GameEventDispatcher, IInitable
    {
        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private InventoryService _inventoryService;

        [Inject]
        private BillingService _billingService;

        [Inject]
        private PlayerResourceModel _resourceModel;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        private ShopDescriptor _shopDescriptor;

        public void Init()
        {
            _shopDescriptor = new ShopDescriptor();
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

        [NotNull]
        public ShopDescriptor GetDescriptor()
        {
            return _shopDescriptor == null ? throw new NullReferenceException("ShopDescriptor is null") : _shopDescriptor;
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