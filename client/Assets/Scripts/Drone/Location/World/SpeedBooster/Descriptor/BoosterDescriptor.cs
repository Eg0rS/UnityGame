using System.Collections.Generic;
using AgkCommons.Configurations;

namespace Drone.Location.World.SpeedBooster.Descriptor
{
    public class BoosterDescriptor
    {
        private string _id;
        public Dictionary<string, float> _params = new Dictionary<string, float>();

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");

            for (int i = 1; i < config.GetNames().Count; i++) {
                string key = config.GetNames()[i];
                float value = config.GetList<float>("boosters")[i];
                _params.Add(key, value);
            }
        }

        public string Id
        {
            get => _id;
            private set => _id = value;
        }
    }
}