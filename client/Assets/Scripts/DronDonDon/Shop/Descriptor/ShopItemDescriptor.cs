using System.Xml.Serialization;
using AgkCommons.Configurations;
using DronDonDon.Inventory.Model;

namespace DronDonDon.Shop.Descriptor
{
    public class ShopItemDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        
        [XmlAttribute("type")]
        public InventoryItemTypeModel Type { get; set; } 
        
        [XmlAttribute("price")]
        public int Price { get; set; }
        
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlAttribute("model")]
        public string Model { get; set; }
        
        public void Configure(Configuration configItem)
        {
            Id = configItem.GetString("id");
            int type = configItem.GetInt("type");
            Type = (InventoryItemTypeModel)type;
            Model = configItem.GetString("model");
            Price = configItem.GetInt("price");
            Name = configItem.GetString("name");
        }
    }
}