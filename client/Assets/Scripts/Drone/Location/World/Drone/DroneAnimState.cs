using System.Runtime.Serialization;

namespace Drone.Location.Service.Control.Drone
{
    [DataContract]
    public enum DroneAnimState
    {
        amEnableShield,
        amDisableShield,
        amEnableSpeed,
        amDisableSpeed,
    }
}