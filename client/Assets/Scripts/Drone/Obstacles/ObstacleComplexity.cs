using System.Xml.Serialization;

namespace Drone.Obstacles
{
    public enum ObstacleComplexity
    {
        [XmlEnum("none")]
        NONE,
        [XmlEnum("easy")]
        EASY,
        [XmlEnum("normal")]
        NORMAL,
        [XmlEnum("hard")]
        HARD,
        [XmlEnum("impossible")]
        IMPOSSIBLE,
    }
}