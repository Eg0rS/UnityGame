using AgkCommons.Configurations;

namespace DronDonDon.Game.Levels.Descriptor
{
    public class LevelDescriptor
    {
        public string Id { get;  set; }
        public string Prefab { get; private set; }
        public int Order { get; private set; }
        
        //TODO здесь ещё должны быть мин.время, кол-во чипов на уровне
        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Prefab = config.GetString("prefab");
            Order = config.GetInt("order");
        }
    }
}