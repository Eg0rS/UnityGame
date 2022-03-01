﻿using Drone.Location.Service.Control.Drone.Descriptor;

namespace Drone.Location.Service.Control.Drone.Model
{
    public class DroneModel
    {
        public float durability;
        public float energy;
        public float mobility;
        public int countChips;
        public float energyFall;
        public float maxDurability;
        public float maxSpeed;
        public float acceleration;
        private DroneDescriptor _droneDescriptor;

        public DroneModel(DroneDescriptor descriptor)
        {
            durability = descriptor.Durability;
            energy = descriptor.Energy;
            mobility = descriptor.Mobility;
            countChips = 0;
            energyFall = 0.2f;
            maxDurability = durability;
            maxSpeed = descriptor.MaxSpeed;
            acceleration = descriptor.Acceleration;
            DroneDescriptor = descriptor;
        }

        public DroneDescriptor DroneDescriptor
        {
            get => _droneDescriptor;
            private set => _droneDescriptor = value;
        }
    }
}