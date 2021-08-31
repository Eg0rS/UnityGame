﻿using System.Collections.Generic;
using DeliveryRush.LevelMap.Levels.Descriptor;

namespace DeliveryRush.LevelMap.Levels.IoC
{
    public class LevelDescriptorRegistry
    {
        private List<LevelDescriptor> _levelDescriptors;

        public List<LevelDescriptor> LevelDescriptors
        {
            get => _levelDescriptors;
            set => _levelDescriptors = value;
        }

        public LevelDescriptorRegistry()
        {
            _levelDescriptors = new List<LevelDescriptor>();
        }
    }
}