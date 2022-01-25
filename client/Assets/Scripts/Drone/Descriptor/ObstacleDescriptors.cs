using System.Xml.Serialization;
using Drone.Obstacles.Descriptor;

namespace Drone.Descriptor
{
    [XmlRoot("obstacles")]
    public class ObstacleDescriptors
    {
        [XmlElement("obstacle")]
        public ObstacleDescriptor[] Obstacles { get; set; }
    }
}