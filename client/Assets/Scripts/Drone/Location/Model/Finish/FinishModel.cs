using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Finish
{
    public class FinishModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.FINISH;
        }
    }
}