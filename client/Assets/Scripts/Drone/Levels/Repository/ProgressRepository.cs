using Drone.Core.Repository;
using Drone.Levels.Model;

namespace Drone.Levels.Repository
{
    public class ProgressRepository : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")
        {
        }
    }
}