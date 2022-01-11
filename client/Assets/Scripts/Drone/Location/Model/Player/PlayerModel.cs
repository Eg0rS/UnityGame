using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Drone
{
    public class PlayerModel : PrefabModel
    {
        public PlayerModel()
        {
            ObjectType = WorldObjectType.PLAYER;
        }
    }
}