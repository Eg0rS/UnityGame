using System.Collections.Generic;
using AgkCommons.Configurations;

namespace Drone.LevelMap.Zones.Descriptor
{
    public class ZoneDescriptor
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public List<string> LevelIds { get; private set; }
        public int CountStars { get; private set; }

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Title = config.GetString("title");
            LevelIds = config.GetList<string>("levelsId.levelId");
            CountStars = config.GetInt("countStars");
        }
    }
}