using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.Spline
{
    public class SplineModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.SPLINE;
        }
    }
}