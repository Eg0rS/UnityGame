using DronDonDon.Core.Repository;
using DronDonDon.Location.Model.Dron;

namespace DronDonDon.Location.World.Dron.Service
{
    public class DronRepository : LocalPrefsSingleRepository<DronModel>
    {
        public DronRepository() : base("dronRepository")    
        {
            
        }
    }
}