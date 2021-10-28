using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Battery
{
    public class BatteryModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.BATTERY;
        }
    }
}