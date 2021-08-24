using DeliveryRush.Core.Repository;
using DeliveryRush.Game.Levels.Model;

namespace DeliveryRush.Game.Levels.Repository
{
    public class ProgressRepository  : LocalPrefsSingleRepository<PlayerProgressModel>
    {
        public ProgressRepository() : base("progressRepository")    
        {
        }
    }
}