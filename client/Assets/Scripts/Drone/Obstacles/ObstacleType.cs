using System.Xml.Serialization;

namespace Drone.Obstacles
{
    public enum ObstacleType
    {
        [XmlEnum("none")]
        NONE,
        [XmlEnum("base")]
        BASE,
        [XmlEnum("column")]
        COLUMN,
        [XmlEnum("wood")]
        WOOD,
        [XmlEnum("tube")]
        TUBE
    }
}