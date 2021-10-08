using AgkCommons.Configurations;

namespace Drone.Location.World.Drone.Descriptor
{
    public class DroneDescriptor
    {
        private string _id;
        private string _title;
        private int _energy;
        private int _durability;
        private float _mobility;
        private float _maxSpeed;
        private float _acceleration;
        private string _prefab;

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Title = config.GetString("title");
            Energy = config.GetInt("energy");
            Durability = config.GetInt("durability");
            Mobility = config.GetFloat("mobility");
            MaxSpeed = config.GetFloat("maxSpeed");
            Acceleration = config.GetFloat("acceleration");
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

        public float Mobility
        {
            get => _mobility;
            private set => _mobility = value;
        }
        public float MaxSpeed
        {
            get => _maxSpeed;
            private set => _maxSpeed = value;
        }
        
        public float Acceleration
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