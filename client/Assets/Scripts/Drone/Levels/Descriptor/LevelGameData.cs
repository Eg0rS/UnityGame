using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelGameData
    {
        [XmlElement("tiles")]
        public Tile Tiles { get; set; }
    }

    public class Tile
    {
        [XmlElement("tile")]
        public TileData[] TileData { get; set; }
    }

    public class TileData
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("type")]
        public LevelTileType Type { get; set; }
    }
}