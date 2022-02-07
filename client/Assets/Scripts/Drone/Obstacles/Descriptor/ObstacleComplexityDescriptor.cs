using System.Xml.Serialization;

namespace Drone.Obstacles.Descriptor
{
    public class ObstacleComplexityDescriptor
    {
        [XmlAttribute("id")]
        public ObstacleComplexity Complexity { get; set; }
        [XmlElement("obstacle")]
        public ObstacleDescriptor[] ObstacleDescriptors { get; set; }
    }
}