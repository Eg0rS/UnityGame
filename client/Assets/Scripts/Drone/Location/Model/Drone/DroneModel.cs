using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Drone
{
    public class DroneModel : PrefabModel
    {
        public DroneModel()
        {
            ObjectType = WorldObjectType.PLAYER;
        }
    }
}