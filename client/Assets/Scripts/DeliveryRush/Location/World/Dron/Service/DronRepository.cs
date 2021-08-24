using DeliveryRush.Core.Repository;
using DeliveryRush.Location.World.Dron.Model;

namespace DeliveryRush.Location.World.Dron.Service
{
    public class DronRepository : LocalPrefsSingleRepository<DronModel>
    {
        public DronRepository() : base("dronRepository")    
        {
            
        }
    }
}