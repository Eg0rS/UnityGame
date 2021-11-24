using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.Event
{
    public class ObstacleEvent : GameEvent
    {
        public const string OBSTACLE_CONTACT = "obstacleContact";
        public const string CRUSH = "crush";
        public const string LETHAL_CRUSH = "lethalCrush";
        public const string DURABILITY_UPDATED = "damageUpdated";

        public ContactPoint[] ContactPoints { get; private set; }
        public float ImmersionDepth { get; private set; }
        public float DurabilityValue { get; private set; }

        public ObstacleEvent(string name, ContactPoint[] contactPoints, float immersionDepth) : base(name)
        {
            ContactPoints = contactPoints;
            ImmersionDepth = immersionDepth;
        }

        public ObstacleEvent(string name, float durability) : base(name)
        {
            DurabilityValue = durability;
        }

        public ObstacleEvent(string name) : base(name)
        {
        }
    }
}