using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Spline
{
    public class SplineWalkerModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SPINLINE_WALKER;
        }
    }
}