﻿using Drone.LevelMap.Levels.Descriptor;

namespace Drone.LevelMap.Levels.Model
{
    public class LevelViewModel
    {
        private LevelProgress _levelProgress;
        private LevelDescriptor _levelDescriptor;

        public LevelProgress LevelProgress
        {
            get => _levelProgress;
            set => _levelProgress = value;
        }

        public LevelDescriptor LevelDescriptor
        {
            get => _levelDescriptor;
            set => _levelDescriptor = value;
        }
    }
}