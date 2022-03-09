using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.ChipModel
{
    public class ChipModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.CHIP;
        }
    }
}