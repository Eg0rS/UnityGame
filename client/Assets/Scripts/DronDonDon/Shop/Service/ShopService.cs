using System;
using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using DronDonDon.Billing.Model;
using DronDonDon.Core.Filter;
using DronDonDon.Inventory.Service;
using IoC.Attribute;
using DronDonDon.Shop.Descriptor;
using DronDonDon.Shop.Event;

namespace DronDonDon.Shop.Service
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
        private PlayerResourceModel _resourceModel;

        public void Init()
        {
            _resourceService.LoadConfiguration("Configs/shop@embeded", OnConfigLoaded);
        }

        public void BuyDron(string itemId)
        {
            ShopItemDescriptor shopItemDescriptor = _shopDescriptor.GetShopItem(itemId);
            if (shopItemDescriptor == null) {
                throw new Exception("ShopItem not found, itemId = " + itemId);
            }
            if (_resourceModel.creditsCount >= shopItemDescriptor.Price)
            {
                _inventoryService.AddInventory(itemId);
            }
            else
            {
                // show billing dialog
                //_dialogManager.Require().Show<information dialog>();
            }
        }

        public void ev()
        {
            Dispatch(new ShopEvent(ShopEvent.UPDATED));
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("shop.shopItem"))
            {
                ShopItemDescriptor shopItemDescriptor = new ShopItemDescriptor();
                shopItemDescriptor.Configure(temp);
                _shopDescriptor.ShopItemDescriptors.Add(shopItemDescriptor);
            }
            
        }
    }
}