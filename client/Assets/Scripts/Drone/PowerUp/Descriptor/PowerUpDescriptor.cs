using System;
using System.Collections.Generic;
using System.Data;
using AgkCommons.Configurations;
using JetBrains.Annotations;

namespace Drone.PowerUp.Descriptor
{
    public class PowerUpDescriptor
    {
        private string _id;
        private PowerUpType _type;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();

        public void Configure(Configuration configuration)
        {
            Id = configuration.GetString("id");
            Type = (PowerUpType) Enum.Parse(typeof(PowerUpType), configuration.GetString("type"));
            foreach (Configuration config in configuration.GetList<Configuration>("parameters.parametr")) {
                _parameters.Add(config.GetString("key"), config.GetString("value"));
            }
        }

        public string Id
        {
            get { return _id; }
            private set { _id = value; }
        }
        public PowerUpType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
            private set { _parameters = value; }
        }
        [NotNull]
        public string GetParameterValue(string parametrName)
        {
            string value = Parameters[parametrName];
            return value == null ? throw new NoNullAllowedException($"Parametr {parametrName} in Parameters  is not found") : value;
        }
    }
}