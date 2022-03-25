using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.CutScene
{
    public class CutScenesModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.CUT_SCENES;
        }
    }
}