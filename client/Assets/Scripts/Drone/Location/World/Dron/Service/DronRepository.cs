using Drone.Core.Repository;
using Drone.Location.World.Dron.Model;

namespace Drone.Location.World.Dron.Service
{
    public class DronRepository : LocalPrefsSingleRepository<DronModel>
    {
        public DronRepository() : base("dronRepository")
        {
        }
    }
}