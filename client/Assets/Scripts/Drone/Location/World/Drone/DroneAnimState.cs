using System.Runtime.Serialization;

namespace Drone.Location.World.Drone
{
    [DataContract]
    public enum AnimState
    {
        amEnableShield,
        amDisableShield,
        amEnableSpeed,
        amDisableSpeed,
        amIdle,
        amMoveRight,
        amMoveLeft,
        amMoveUp,
        amMoveDown,
        amMoveUpRight,
        amMoveUpLeft,
        amMoveDownRight,
        amMoveDownLeft
    }
}