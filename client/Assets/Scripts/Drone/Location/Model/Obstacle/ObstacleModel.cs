using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Obstacle
{
    public class ObstacleModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.OBSTACLE;
        }
    }
}