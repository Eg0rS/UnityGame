using System.Xml.Serialization;

namespace Drone.Inventory.Model
{
    public enum InventoryItemTypeModel
    {
        [XmlEnum("dron")]
        DRON,
    }
}