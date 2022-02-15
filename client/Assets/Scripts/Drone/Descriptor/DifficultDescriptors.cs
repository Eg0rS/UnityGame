using System.Xml.Serialization;
using Drone.LevelDifficult.Descriptor;

namespace Drone.Descriptor
{
    [XmlRoot("difficulty")]
    public class DifficultDescriptors
    {
        [XmlElement("difficult")]
        public DifficultDescriptor[] Descriptors { get; set; }
    }
}