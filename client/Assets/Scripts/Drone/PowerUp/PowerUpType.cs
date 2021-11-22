using System.Runtime.Serialization;

namespace Drone.PowerUp
{
    [DataContract]
    public enum PowerUpType
    {
        NONE,
        ACCELERATOR,
        FORCE_FIELD,
        MAGNET,
        ROCKET,
        DOUBLE_CHIPS
    }
}