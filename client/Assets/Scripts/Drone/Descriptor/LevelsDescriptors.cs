using System.Xml.Serialization;
using Drone.Levels.Descriptor;

namespace Drone.Descriptor
{
    [XmlRoot("levels")]
    public class LevelsDescriptors
    {
        [XmlElement("level")]
        public LevelDescriptor[] Levels { get; set; }
    }
}