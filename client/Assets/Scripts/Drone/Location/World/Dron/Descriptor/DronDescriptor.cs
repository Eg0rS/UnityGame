﻿using AgkCommons.Configurations;

namespace Drone.Location.World.Dron.Descriptor
{
    public class DronDescriptor
    {
        private string _id;
        private string _title;
        private int _energy;
        private int _durability;
        private int _mobility;
        private int _maxSpeed;
        private int _acceleration;
        private string _prefab;

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Title = config.GetString("title");
            Energy = config.GetInt("energy");
            Durability = config.GetInt("durability");
            Mobility = config.GetInt("mobility");
            MaxSpeed = config.GetInt("maxSpeed");
            Acceleration = config.GetInt("acceleration");
            Prefab = config.GetString("prefab");
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        private string Title
        {
            set => _title = value;
        }

        public int Energy
        {
            get => _energy;
            private set => _energy = value;
        }

        public int Durability
        {
            get => _durability;
            private set => _durability = value;
        }

        public int Mobility
        {
            get => _mobility;
            private set => _mobility = value;
        }
        public int MaxSpeed
        {
            get => _maxSpeed;
            private set => _maxSpeed = value;
        }
        
        public int Acceleration
        {
            get => _acceleration;
            private set => _acceleration = value;
        }

        public string Prefab
        {
            get => _prefab;
            private set => _prefab = value;
        }
    }
}