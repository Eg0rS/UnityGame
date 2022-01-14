using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("order")]
        public int Order { get; set; }
        [XmlAttribute("type")]
        public LevelType Type { get; set; }

        [XmlElement("game_data")]
        public LevelGameData GameData { get; set; }

        // [XmlElement("goals")]
        // public LevelGoals Goals { get; set; }

        [XmlElement("exposition")]
        public LevelExposition Exposition { get; set; }
    }
}