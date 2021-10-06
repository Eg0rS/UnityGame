using Drone.Location.Model.BaseModel;


namespace Drone.Location.Model.SpeedBooster
{
    public class SpeedBoosterModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SPEED_BOOSTER;

        }
    }
}