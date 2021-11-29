using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelGraphics
    {
        [XmlAttribute("prefab")]
        public string Prefab { get; set; }
        [XmlAttribute("image")]
        public string Image { get; set; }
    }
}