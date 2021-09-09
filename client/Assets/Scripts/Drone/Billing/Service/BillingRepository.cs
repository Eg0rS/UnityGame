using Drone.Billing.Model;
using Drone.Core.Repository;

namespace Drone.Billing.Service
{
    public class BillingRepository : LocalPrefsSingleRepository<PlayerResourceModel>
    {
        public BillingRepository() : base("creditShopRepository")
        {
        }
    }
}