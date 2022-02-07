using System.Xml.Serialization;

namespace Drone.Obstacles.Descriptor
{
    public class ObstacleTypeDescriptor
    {
        [XmlAttribute("id")]
        public ObstacleType Type { get; set; }
        [XmlElement("complexity")]
        public ObstacleComplexityDescriptor[] ObstacleComplexity { get; set; }
    }
}