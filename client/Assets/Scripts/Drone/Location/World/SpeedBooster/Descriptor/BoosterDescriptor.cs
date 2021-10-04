using System;
using System.Collections.Generic;
using AgkCommons.Configurations;

namespace Drone.Location.World.SpeedBooster.Descriptor
{
    public class BoosterDescriptor
    {
        private string _id;
        private string _type;
        public Dictionary<string, object> _params = new Dictionary<string, object>();

        public void Configure(Configuration config)
        {
            Id = config.GetString("id");
            Type = config.GetString("type");
            foreach (Configuration conf in config.GetList<Configuration>("params.param")) {
                _params.Add(conf.GetString("key"), conf.GetString("value"));
            }
        }

        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }
        public string Type
        {
            get { return _type; }
            private set { _type = value; }
        }
    }
}