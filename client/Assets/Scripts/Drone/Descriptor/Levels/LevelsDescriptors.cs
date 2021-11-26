using System.Xml.Serialization;

namespace Drone.Descriptor.Levels
{
    [XmlRoot("levels")]
    public class LevelsDescriptors
    {
        [XmlElement("level")]
        public LevelDescriptor[] Levels { get; set; }
    }
}