using System.Xml.Serialization;
using Tile.Descriptor;

namespace Drone.Descriptor
{
    [XmlRoot("tiles")]
    public class TileDescriptors
    {
        [XmlElement("tile")]
        public TileDescriptor[] Tiles { get; set; }
    }
}