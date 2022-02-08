using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelTask
    {
        [XmlAttribute("description")]
        public string Description { get; set; }
    }
}