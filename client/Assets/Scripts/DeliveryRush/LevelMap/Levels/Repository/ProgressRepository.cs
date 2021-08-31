using DeliveryRush.Core.Repository;
using DeliveryRush.LevelMap.Levels.Model;

namespace DeliveryRush.LevelMap.Levels.Repository
{
    public class ProgressRepository : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")
        {
        }
    }
}