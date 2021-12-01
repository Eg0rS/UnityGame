using AgkCommons.Configurations;
using AgkCommons.Event;
using AgkCommons.Resources;
using AgkUI.Dialog.Service;
using Drone.Billing.Descriptor;
using Drone.Billing.Event;
using Drone.Billing.IoC;
using Drone.Billing.Model;
using Drone.Core.Service;
using Drone.Shop.UI;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Billing.Service
{
    public class BillingService : GameEventDispatcher, IConfigurable
    {
        [Inject] private BillingRepository _billingRepository;

        [Inject] private ResourceService _resourceService;

        [Inject] private BillingDescriptorRegistry _billingDescriptorRegistry;

        [Inject] private IoCProvider<DialogManager> _dialogManager;

        private PlayerResourceModel _resourceModel;

        public void Configure()
        {
            _resourceModel = RequirePlayerResourceModel();
            _resourceService.LoadConfiguration("Configs/Billing@embeded", OnConfigLoaded);
        }

        private void OnConfigLoaded(Configuration config, object[] loadparameters)
        {
            foreach (Configuration temp in config.GetList<Configuration>("billing.billingItem"))
            {
                BillingDescriptor shopItemDescriptor = new BillingDescriptor();
                shopItemDescriptor.Configure(temp);
                _billingDescriptorRegistry.BillingDescriptors.Add(shopItemDescriptor);
            }
        }

        public PlayerResourceModel RequirePlayerResourceModel()
        {
            PlayerResourceModel model = _billingRepository.Get();
            if (model == null)
            {
                InitPlayerResourceModel();
            }
            return _billingRepository.Require();
        }

        private bool HasCreditShopModel()
        {
            return (_billingRepository.Get() != null);
        }

        private void InitPlayerResourceModel()
        {
            PlayerResourceModel playerResourceModel = new PlayerResourceModel
            {
                CreditsCount = 0,
                CryptoCount = 0
            };
            _billingRepository.Set(playerResourceModel);
        }

        public void SetCreditsCount(int count)
        {
            _resourceModel.CreditsCount = count;
            _billingRepository.Set(_resourceModel);
            Dispatch(new BillingEvent(BillingEvent.UPDATED));
        }

        public void SetCryptoCount(int count)
        {
            _resourceModel.CryptoCount = count;
            _billingRepository.Set(_resourceModel);
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

        public void ShowDroneStoreDialog()
        {
            _dialogManager.Require().Show<ShopDialog>();
        }
    }
}