using System.Xml.Serialization;

namespace Drone.Obstacles
{
    public enum ObstacleType
    {
        [XmlEnum("wood")]
        WOOD,
        [XmlEnum("column")]
        COLUMN
    }
}