using Drone.Core.Repository;
using Drone.LevelMap.Levels.Model;

namespace Drone.Levels.Repository
{
    public class ProgressRepository : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")
        {
        }
    }
}