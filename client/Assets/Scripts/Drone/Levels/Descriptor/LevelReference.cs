using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelReference
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
    }
}