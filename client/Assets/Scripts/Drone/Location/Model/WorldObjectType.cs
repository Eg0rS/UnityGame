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
        CUT_SCENES,
        START_PLATFORM,
        GEOMETRY_ROTATION
    }
}