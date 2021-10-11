using System.Runtime.Serialization;

namespace Drone.Location.World.Drone
{
    [DataContract]
    public enum AnimState
    {
        EnableShield,
        DisableShield,
        EnableSpeed,
        DisableSpeed
    }
}