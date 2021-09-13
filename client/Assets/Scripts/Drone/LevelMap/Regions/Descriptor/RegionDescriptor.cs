using System.Collections.Generic;
using AgkCommons.Configurations;

namespace Drone.LevelMap.Regions.Descriptor
{
    public class RegionDescriptor
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public List<string> LevelId { get; private set; }
        public int CountStars { get; private set; }

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Title = config.GetString("title");
            LevelId = config.GetList<string>("levelsId.levelId");
            CountStars = config.GetInt("countStars");
        }
    }
}