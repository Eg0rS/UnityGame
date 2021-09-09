using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Battery
{
    public class BatteryModel : PrefabModel
    {
        public float Energy { get; private set; }

        public void Awake()
        {
            ObjectType = WorldObjectType.Battery;
            Energy = 3f;
        }
    }
}