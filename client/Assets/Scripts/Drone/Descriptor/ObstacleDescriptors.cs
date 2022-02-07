using System.Xml.Serialization;
using Drone.Obstacles.Descriptor;

namespace Drone.Descriptor
{
    [XmlRoot("obstacle_types")]
    public class ObstacleDescriptors
    {
        [XmlElement("type")]
        public ObstacleTypeDescriptor[] Types { get; set; }
    }
}