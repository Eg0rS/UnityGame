using System.Xml.Serialization;

namespace DronDonDon.Shop.Descriptor
{
    public class ShopItemDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        
        [XmlAttribute("type")]
        public string Type { get; set; } // 1 тип предметов - дроны
        
        [XmlAttribute("price")]
        public int Price { get; set; }
        
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}