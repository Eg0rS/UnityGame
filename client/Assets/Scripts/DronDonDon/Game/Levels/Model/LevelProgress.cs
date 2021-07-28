﻿using UnityEngine;

namespace DronDonDon.Game.Levels.Model
{
    public class LevelProgress
    {
        private string _id;
        private int _transitTime;
        private int _countStars;
        private int _countChips;
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public int TransitTime
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
    }
}