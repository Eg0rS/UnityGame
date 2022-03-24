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
        [XmlElement("exposition")]
        public LevelExposition Exposition { get; set; }
        [XmlAttribute("zone")]
        public LevelZoneType Zone { get; set; }
        [XmlAttribute("version")]
        public int Version { get; set; }

        [XmlAttribute("revardForPassing")]
        public int RewardForPassing { get; set; }
        [XmlAttribute("chipsForPassing")]
        public int ChipsForPassing { get; set; }
        
    }
}