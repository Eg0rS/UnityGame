using System.Xml.Serialization;

namespace Tile.Descriptor
{
    public enum TileType
    {
        [XmlEnum("start")]
        START,
        [XmlEnum("finish")]
        FINISH,
        [XmlEnum("regular")]
        REGULAR,
        [XmlEnum("bonus")]
        BONUS
    }
}