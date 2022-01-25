using System.Xml.Serialization;

namespace Tile.Descriptor
{
    public enum TileType
    {
        [XmlEnum("start")]
        START,
        [XmlEnum("end")]
        END,
        [XmlEnum("regular")]
        REGULAR,
        [XmlEnum("bonus")]
        BONUS
    }
}