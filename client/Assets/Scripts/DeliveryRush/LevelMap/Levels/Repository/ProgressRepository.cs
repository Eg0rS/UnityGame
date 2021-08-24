using DeliveryRush.Core.Repository;
using DeliveryRush.Resource.Model;

namespace DeliveryRush.Resource.Repository
{
    public class ProgressRepository  : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")    
        {
        }
    }
}