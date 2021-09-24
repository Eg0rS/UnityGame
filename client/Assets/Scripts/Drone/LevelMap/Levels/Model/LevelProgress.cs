﻿namespace Drone.LevelMap.Levels.Model
{
    public class LevelProgress
    {
        private string _id;
        private float _transitTime;
        private int _countStars;
        private int _countChips;
        private float _durability;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public float TransitTime
        {
            get => _transitTime;
            set => _transitTime = value;
        }

        public int CountStars
        {
            get => _countStars;
            set => _countStars = value;
        }

        public int CountChips
        {
            get => _countChips;
            set => _countChips = value;
        }

        public float Durability
        {
            get => _durability;
            set => _durability = value;
        }
    }
}