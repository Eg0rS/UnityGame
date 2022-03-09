using System.Runtime.Serialization;

namespace Drone.Location.Model
{
    [DataContract]
    public enum WorldObjectType
    {
        NONE,
        PLAYER,
        OBSTACLE,
        CHIP,
        FINISH,
        SPLINE_WALKER,
        SPLINE,
        SPAWNER,
        TILE,
        START_PLATFORM,
        GEOMETRY_ROTATION
    }
}