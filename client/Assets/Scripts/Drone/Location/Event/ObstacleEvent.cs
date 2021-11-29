using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.Event
{
    public class ObstacleEvent : GameEvent
    {
        public const string OBSTACLE_CONTACT = "obstacleContact";
        public const string CRUSH = "crush";

        public ContactPoint[] ContactPoints { get; private set; }
        public float ImmersionDepth { get; private set; }

        public float DurabilityDelta { get; private set; }
        public bool IsLethalCrush { get; private set; }

        public ObstacleEvent(string name, ContactPoint[] contactPoints, float immersionDepth) : base(name)
        {
            ContactPoints = contactPoints;
            ImmersionDepth = immersionDepth;
        }

        public ObstacleEvent(string name, bool isLethalCrush, float durability) : base(name)
        {
            DurabilityDelta = durability;
            IsLethalCrush = isLethalCrush;
        }
    }
}