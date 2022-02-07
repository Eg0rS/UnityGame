using System.Linq;
using System.Xml.Serialization;
using Drone.Levels.Descriptor;
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
        public LevelZoneType Zone { get; set; }
        [XmlAttribute("type")]
        public TileType Type { get; set; }
        // [XmlAnyElement("obstacle_types")]
        // private ObstacleType[] _obstacleTypes;

        [XmlElement("")]
        public ObstacleType[] ObstacleTypes { get; set; }

        [XmlElement("obstacle_types")]
        public Obs[] ObstacleTypes1 { get; set; }
    }

    public class Obs
    {
        [XmlArrayItem("type")]
        public string type;
    }
}