using System.Xml.Serialization;

namespace DeliveryRush.Inventory.Model
{
    public enum InventoryItemTypeModel
    {
        [XmlEnum("dron")]
        DRON,
    }
}