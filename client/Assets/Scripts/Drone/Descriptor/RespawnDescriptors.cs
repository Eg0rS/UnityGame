using System.Xml.Serialization;

namespace Drone.Descriptor
{
    [XmlRoot("respawns")]
    public class RespawnDescriptors
    {
        [XmlElement("respawn")]
        public RespawnDescriptor[] Descriptors { get; set; }
    }

    public class RespawnDescriptor
    {
        [XmlAttribute("collisionId")]
        public int CollisionCount { get; set; }
        [XmlAttribute("price")]
        public int Price { get; set; }
    }
}