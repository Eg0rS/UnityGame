using System.Xml.Serialization;

namespace Drone.Levels.Descriptor
{
    public class LevelGoals
    {
        [XmlAttribute("chips")]
        public int NecessaryCountChips { get; set; }
        [XmlAttribute("time")]
        public int NecessaryTime { get; set; }
        [XmlAttribute("durability")]
        public int NecessaryDurability { get; set; }
    }
}