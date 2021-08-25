using DeliveryRush.Billing.Model;
using DeliveryRush.Core.Repository;

namespace DeliveryRush.Billing.Service
{
    public class BillingRepository : LocalPrefsSingleRepository<PlayerResourceModel>
    {
        public BillingRepository() : base("creditShopRepository")
        {
        }
    }
}