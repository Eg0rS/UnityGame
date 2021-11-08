using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.X2Booster
{
    public class X2BoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.X2_BOOSTER;
        }
    }
}