using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Magnet
{
    public class MagnetBoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.MAGNET_BOOSTER;
        }
    }
}