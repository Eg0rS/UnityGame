using Drone.Booster.Service;
using Drone.Location.Model.BaseModel;
using IoC.Attribute;

namespace Drone.Location.Model.ShieldBooster
{
    public class ShieldBoosterModel : PrefabModel
    {
        public float Duration { get; private set; }
        public void Awake()
        {
            ObjectType = WorldObjectType.SHIELD_BOOSTER;
           
        }
    }
}