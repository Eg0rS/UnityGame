using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.Event
{
    public class WorldEvent : GameEvent
    {
        public const string CREATED = "worldCreated";
        public const string PAUSED = "worldPaused";
        public const string UNPAUSED = "worldUnPaused";
        public const string DESTROYED = "worldDestroyed";
        public const string ENABLED = "worldEnabled";

        private readonly bool _enabled;

        public WorldEvent(string name, GameObject target) : base(name, target)
        {
        }

        public WorldEvent(string name, bool enabled) : base(name)
        {
            _enabled = enabled;
        }

        public WorldEvent(string name) : base(name)
        {
        }

        public bool Enabled
        {
            get { return _enabled; }
        }
    }
}