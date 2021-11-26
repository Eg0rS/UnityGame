using System.Xml.Serialization;

namespace Drone.Descriptor.Levels
{
    public class LevelDescriptor
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("prefab")]
        public string Prefab { get; set; }
        [XmlAttribute("order")]
        public int Order { get; set; }
        [XmlAttribute("chips")]
        public int NecessaryCountChips { get; set; }
        [XmlAttribute("time")]
        public int NecessaryTime { get; set; }
        [XmlAttribute("durability")]
        public int NecessaryDurability { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("image")]
        public string Image { get; set; }
        [XmlAttribute("type")]
        public LevelType Type { get; set; }
    }
}