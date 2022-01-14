using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public enum LevelTileType
    {
        [XmlEnum("start")]
        START,
        [XmlEnum("end")]
        END,
        [XmlEnum("regular")]
        REGULAR,
        [XmlEnum("bonus")]
        BONUS
    }
}