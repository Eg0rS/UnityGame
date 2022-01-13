using System.Xml.Serialization;

namespace Drone.Obstacles.Descriptor
{
    public class ObstacleDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("type")]
        public ObstacleType Type { get; set; }
    }
}