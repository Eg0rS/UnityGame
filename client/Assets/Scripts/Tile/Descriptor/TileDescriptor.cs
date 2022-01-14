using System.Linq;
using System.Xml.Serialization;
using Drone.Levels.Descriptor;
using Drone.Obstacles;
using JetBrains.Annotations;

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
        private ObstacleType[] _obstacleTypes;
        [XmlElement("obstacle_type")]
        [NotNull]
        public ObstacleType[] ObstacleTypes
        {
            get { return _obstacleTypes.Distinct().ToArray(); }
            set { _obstacleTypes = value; }
        }
    }
}