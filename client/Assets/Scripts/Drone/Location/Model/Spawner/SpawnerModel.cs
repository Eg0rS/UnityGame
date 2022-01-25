using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Spawner
{
    public class SpawnerModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SPAWNER;
        }
    }
}