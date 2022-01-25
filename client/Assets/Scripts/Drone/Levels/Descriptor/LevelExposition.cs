using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelExposition
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("image")]
        public string Image { get; set; }
    }
}