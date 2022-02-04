using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelGameData
    {
        [XmlElement("tiles")]
        public Tiles Tiles { get; set; }
        [XmlElement("task")]
        public LevelTask[] LevelTasks { get; set; }

    }

    public class Tiles
    {
        [XmlElement("tile")]
        public TileData[] TilesData { get; set; }
    }

    public class TileData
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
    
}