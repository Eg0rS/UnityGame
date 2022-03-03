using System.Xml.Serialization;
using Drone.Levels.Descriptor;

namespace Tile.Descriptor
{
    public class TileDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("prefab")]
        public string Prefab { get; set; }
        [XmlAttribute("zone")]
        public LevelZoneType Zone { get; set; }
        [XmlAttribute("type")]
        public TileType Type { get; set; }

        [XmlArray("obstacle_types")]
        [XmlArrayItem("type")]
        public string[] ObstacleTypes { get; set; }
        
        [XmlArray("red_zones")]
        [XmlArrayItem("zone")]
        public Zone[] RedZones { get; set; }
    }

    public class Zone
    {
        [XmlAttribute("begin")]
        public float Begin { get; set; }
        [XmlAttribute("end")]
        public float End { get; set; }
    }
}