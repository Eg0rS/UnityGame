using System.Runtime.Serialization;

namespace Drone.Location.Model
{
    [DataContract]
    public enum WorldObjectType
    {
        NONE,
        DRON,
        OBSTACLE,
        BATTERY,
        BONUS_CHIPS,
        SPEED_BOOSTER,
        SHIELD_BOOSTER,
        X2_BOOSTER,
        MAGNET_BOOSTER,
        FINISH
    }
}