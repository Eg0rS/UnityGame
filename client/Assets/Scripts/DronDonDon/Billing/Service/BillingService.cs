using AgkCommons.Event;
using IoC.Attribute;
using DronDonDon.Billing.Model;
using DronDonDon.Billing.Event;

namespace DronDonDon.Billing.Service
{
    public class BillingService : GameEventDispatcher
    {
        [Inject]
        private BillingRepository _creditShopRepository; 
        
        public void UpdateSettings()
        {
            BillingModel billingModel = RequireCreditShopModel();
            SetCreditsCount(billingModel.creditsCount);
        }
        
        public BillingModel RequireCreditShopModel()
        {
            BillingModel model = _creditShopRepository.Get();
            if (model == null)
            {
                InitCreditsCount();
            }
            return _creditShopRepository.Require();
        }
        
        public bool HasCreditShopModel()
        {
            return (_creditShopRepository.Get() != null);
        }
        public void InitCreditsCount()
        {
            if (!HasCreditShopModel()) {
                BillingModel billingModel = new BillingModel();
                billingModel.creditsCount = 0;
                _creditShopRepository.Set(billingModel);
                
                
            }
            UpdateSettings();
        }
        public void SetCreditsCount(int count)
        {
            BillingModel creditShopModel = RequireCreditShopModel();
            creditShopModel.creditsCount = count;
            _creditShopRepository.Set(creditShopModel);
            Dispatch(new BillingEvent(BillingEvent.UPDATED));
        }
        public int GetCreditsCount()
        {
            return RequireCreditShopModel().creditsCount;
        }
    }
}