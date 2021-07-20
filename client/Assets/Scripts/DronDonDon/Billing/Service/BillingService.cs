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
            PlayerResourceModel playerResourceModel = RequirePlayerResourceModel();
            SetCreditsCount(playerResourceModel.creditsCount);
        }
        
        public PlayerResourceModel RequirePlayerResourceModel()
        {
            PlayerResourceModel model = _creditShopRepository.Get();
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
                PlayerResourceModel playerResourceModel = new PlayerResourceModel();
                playerResourceModel.creditsCount = 0;
                _creditShopRepository.Set(playerResourceModel);
            }
            UpdateSettings();
        }
        public void SetCreditsCount(int count)
        {
            PlayerResourceModel playerResourceModel = RequirePlayerResourceModel();
            playerResourceModel.creditsCount = count;
            _creditShopRepository.Set(playerResourceModel);
            Dispatch(new BillingEvent(BillingEvent.UPDATED));
        }
        public int GetCreditsCount()
        {
            return RequirePlayerResourceModel().creditsCount;
        }

        public void AddCredits(int count)
        {
            SetCreditsCount(GetCreditsCount()+count);
        }
    }
}