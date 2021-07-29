using AgkCommons.Configurations;

namespace DronDonDon.Game.Levels.IoC
{
    public class LevelDescriptor
    {
        public string Id { get;  set; }
        public string Prefab { get; private set; }
        public int Order { get; private set; }
        
        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Prefab = config.GetString("prefab");
            Order = config.GetInt("order");
        }
    }
}