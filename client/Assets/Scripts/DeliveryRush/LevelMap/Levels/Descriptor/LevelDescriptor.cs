using AgkCommons.Configurations;

namespace DeliveryRush.LevelMap.Levels.Descriptor
{
    public class LevelDescriptor
    {
        public string Id { get; private set; }
        public string Prefab { get; private set; }
        public int Order { get; private set; }
        public int NecessaryCountChips { get; private set; }
        public int NecessaryTime { get; private set; }
        public int NecessaryDurability { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Image { get; private set; }
        public LevelType Type { get; private set; }

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Prefab = config.GetString("prefab");
            Order = config.GetInt("order");
            NecessaryCountChips = config.GetInt("chips");
            NecessaryTime = config.GetInt("time");
            NecessaryDurability = config.GetInt("durability");
            Title = config.GetString("title");
            Description = config.GetString("description");
            Image = config.GetString("image");
            int type = config.GetInt("type");
            Type = (LevelType) type;
        }
    }
}