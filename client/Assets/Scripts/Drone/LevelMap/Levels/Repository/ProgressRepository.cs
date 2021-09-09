using Drone.Core.Repository;
using Drone.LevelMap.Levels.Model;

namespace Drone.LevelMap.Levels.Repository
{
    public class ProgressRepository : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")
        {
        }
    }
}