using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Player
{
    public class PlayerModel : PrefabModel
    {
        public PlayerModel()
        {
            ObjectType = WorldObjectType.PLAYER;
        }
    }
}