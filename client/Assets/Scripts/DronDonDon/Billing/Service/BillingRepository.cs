using DronDonDon.Billing.Model;
using DronDonDon.Core.Repository;

namespace DronDonDon.Billing.Service
{
    public class BillingRepository: LocalPrefsSingleRepository<BillingModel>
    {
        public BillingRepository() : base("creditShopRepository")    
        {
            
        }
    }
}