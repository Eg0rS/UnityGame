using AgkCommons.Configurations;

namespace Drone.Location.World.Player.Descriptor
{
    public class DroneDescriptor
    {
        private string _id;
        private string _title;
        private float _mobility;
        private string _prefab;

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Title = config.GetString("title");
            Mobility = config.GetFloat("mobility");
            Prefab = config.GetString("prefab");
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public float Mobility
        {
            get { return _mobility; }
            private set { _mobility = value; }
        }

        public string Prefab
        {
            get { return _prefab; }
            private set { _prefab = value; }
        }
    }
}