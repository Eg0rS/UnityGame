using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.StartPlatform
{
    public class StartPlatformModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.START_PLATFORM;
        }
    }
}