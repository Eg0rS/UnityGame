using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using Drone.Billing.Descriptor;
using Drone.Billing.Event;
using Drone.Billing.IoC;
using Drone.Billing.Model;
using Drone.Core.Filter;
using Drone.Shop.UI;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Billing.Service
{
    public class BillingService : GameEventDispatcher, IInitable
    {
        [Inject]
        private BillingRepository _creditShopRepository;

        [Inject]
        private ResourceService _resourceService;

        [Inject]
        private BillingDescriptorRegistry _billingDescriptorRegistry;

        [Inject]
        private IoCProvider<DialogManager> _dialogManager;

        private PlayerResourceModel _resourceModel;
        
        public void Init()
        {
            _resourceModel = RequirePlayerResourceModel();
            _resourceService.LoadConfiguration("Configs/Billing@embeded", OnConfigLoaded);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("billing.billingItem")) {
                BillingDescriptor shopItemDescriptor = new BillingDescriptor();
                shopItemDescriptor.Configure(temp);
                _billingDescriptorRegistry.BillingDescriptors.Add(shopItemDescriptor);
            }
        }

        public PlayerResourceModel RequirePlayerResourceModel()
        {
            PlayerResourceModel model = _creditShopRepository.Get();
            if (model == null) {
                InitCreditsCount();
            }
            return _creditShopRepository.Require();
        }

        private bool HasCreditShopModel()
        {
            return (_creditShopRepository.Get() != null);
        }

        private void InitCreditsCount()
        {
            if (!HasCreditShopModel()) {
                PlayerResourceModel playerResourceModel = new PlayerResourceModel();
                playerResourceModel.CreditsCount = 0;
                playerResourceModel.CryptoCount = 0;
                _creditShopRepository.Set(playerResourceModel);
            }
            SetCreditsCount(_resourceModel.CreditsCount);
            SetCryptoCount(_resourceModel.CryptoCount);
        }

        public void SetCreditsCount(int count)
        {
            _resourceModel.CreditsCount = count;
            _creditShopRepository.Set(_resourceModel);
            Dispatch(new BillingEvent(BillingEvent.UPDATED));
        }
        
        public void SetCryptoCount(int count)
        {
            _resourceModel.CryptoCount = count;
            _creditShopRepository.Set(_resourceModel);
            Dispatch(new BillingEvent(BillingEvent.UPDATED));
        }

        public int GetCreditsCount()
        {
            return _resourceModel.CreditsCount;
        }

        public int GetCryptoCount()
        {
            return _resourceModel.CryptoCount;
        }

        public void AddCredits(int count)
        {
            SetCreditsCount(GetCreditsCount() + count);
        }
        
        public void AddCrypto(int count)
        {
            SetCryptoCount(GetCryptoCount() + count);
        }

        public void ShowDronStoreDialog()
        {
            _dialogManager.Require().Show<ShopDialog>();
        }
    }
}