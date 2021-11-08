using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.ShieldBooster
{
    public class ShieldBoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SHIELD_BOOSTER;
        }
    }
}