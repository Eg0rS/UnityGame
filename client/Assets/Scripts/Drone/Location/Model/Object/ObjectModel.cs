using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Object
{
    public class ObjectModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.OBSTACLE;
        }
    }
}