using System;
using Drone.Location.Model.BaseModel;
using Drone.Location.World.Drone.Descriptor;

namespace Drone.Location.World.Drone.Model
{
    public class DroneModel : PrefabModel
    {
        public float durability;
        public float energy;
        public float mobility;
        public int countChips;
        public float energyFall;
        public float maxDurability;

        public float maxSpeed;
        public float acceleration;
        
        private DronDescriptor _dronDescriptor;
        
        public DroneModel(DronDescriptor descriptor)
        {
            durability = descriptor.Durability;
            energy = descriptor.Energy;
            mobility = descriptor.Mobility;
            countChips = 0;
            energyFall = 0f;
            maxDurability = durability;
            maxSpeed = descriptor.MaxSpeed;
            acceleration = descriptor.Acceleration;
            DronDescriptor = descriptor;
        }

        public DronDescriptor DronDescriptor
        {
            get => _dronDescriptor;
            private set => _dronDescriptor = value;
        }
        
        private void Awake()
        {
            throw new NotImplementedException();
        }
    }
}