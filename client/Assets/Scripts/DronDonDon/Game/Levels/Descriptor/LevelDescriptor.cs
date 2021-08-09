﻿using AgkCommons.Configurations;

namespace DronDonDon.Game.Levels.Descriptor
{
    public class LevelDescriptor
    {
        public string Id { get; private set; }
        public string Prefab { get; private set; }
        public int Order { get; private set; }
        public int NecessaryCountChips { get; private set; }
        public int NecessaryTime { get; private set; }
        public int NecessaryDurability { get; private set; }
        public string LevelTitle { get; private set; }
        public string LevelDescription { get; private set; }
        public string LevelImage { get; private set; }
        
        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Prefab = config.GetString("prefab");
            Order = config.GetInt("order");
            NecessaryCountChips = config.GetInt("chips");
            NecessaryTime = config.GetInt("time");
            NecessaryDurability = config.GetInt("durability");
            LevelTitle = config.GetString("title");
            LevelDescription = config.GetString("description");
            LevelImage = config.GetString("image");
        }
    }
}