using Drone.Location.Model.BaseModel;

namespace Drone.Location.Model.WorldGeomertyRotation
{
    public class WorldGeometryRotationModel : PrefabModel
    {
        public void Awake()
        {
            ObjectType = WorldObjectType.GEOMETRY_ROTATION;
        }
    }
}