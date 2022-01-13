using System.Xml.Serialization;
using Drone.Obstacles;

namespace Tile.Descriptor
{
    public class TileDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("prefab")]
        public string Prefab { get; set; }
        [XmlAttribute("zone")]
        public string Zone { get; set; }
        [XmlElement("obstacle_types")]
        public Type Types { get; set; }
    }

    public class Type
    {
        [XmlElement("type")]
        public Tp[] ype { get; set; }
    }
    public class Tp
    {
        [XmlAttribute("type")]
        public ObstacleType ype { get; set; }
    }
}