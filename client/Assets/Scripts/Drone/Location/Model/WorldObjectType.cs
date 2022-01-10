using System.Runtime.Serialization;

namespace Drone.Location.Model
{
    [DataContract]
    public enum WorldObjectType
    {
        NONE,
        PLAYER,
        OBSTACLE,
        BONUS_CHIPS,
        FINISH,
        SPLINE_WALKER,
        SPLINE,
        SPAWNER
    }
}