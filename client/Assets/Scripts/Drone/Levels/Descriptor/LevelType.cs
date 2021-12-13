using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public enum LevelType
    {
        [XmlEnum("normal")]
        NORMAL,
        [XmlEnum("medium")]
        MEDIUM,
        [XmlEnum("hard")]
        HARD,
        [XmlEnum("boss")]
        BOSS
    }
}