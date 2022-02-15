using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public enum LevelType
    {
        [XmlEnum("none")]
        NONE,
        [XmlEnum("easy")]
        EASY,
        [XmlEnum("normal")]
        NORMAL,
        [XmlEnum("hard")]
        HARD,
        [XmlEnum("boss")]
        BOSS
    }
}