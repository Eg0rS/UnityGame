using System.Xml.Serialization;
using Drone.Levels.Descriptor;

namespace Drone.LevelDifficult.Descriptor
{
    public class DifficultDescriptor
    {
        [XmlAttribute("id")]
        public LevelType DifficultName { get; set; }
        [XmlAttribute("spawn-step")]
        public float SpawnStep { get; set; }
        [XmlAttribute("easy")]
        public float EasySpawnChance { get; set; }
        [XmlAttribute("normal")]
        public float NormalSpawnChance { get; set; }
        [XmlAttribute("hard")]
        public float HardSpawnChance { get; set; }
    }
}