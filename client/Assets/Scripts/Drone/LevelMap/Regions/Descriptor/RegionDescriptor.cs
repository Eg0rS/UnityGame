using System.Collections.Generic;
using System.Security.Cryptography;
using AgkCommons.Configurations;

namespace DeliveryRush.LevelMap.Regions.Descriptor
{
    public class RegionDescriptor
    {
        public string Id { get; private set; }
        public string PreviousRegionId { get; private set; }
        public string Title { get; private set; }
        public List<string> LevelId { get; private set; }
        public int CountStars { get; private set; }

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            PreviousRegionId = config.GetString("previousId");
            Title = config.GetString("title");
            LevelId = config.GetList<string>("levelsId");
            CountStars = config.GetInt("countStars");
        }
    }
}