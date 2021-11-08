using System.Runtime.Serialization;

namespace Drone.Location.World.Drone
{
    [DataContract]
    public enum DroneAnimState
    {
        amEnableShield,
        amDisableShield,
        amEnableSpeed,
        amDisableSpeed,
        amIdle
    }
}