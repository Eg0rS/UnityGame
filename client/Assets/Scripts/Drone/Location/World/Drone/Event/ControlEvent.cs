using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.Service.Control.Drone.Event
{
    public class ControllEvent : GameEvent
    {
        public const string GESTURE = "gesture";
        public Vector2 Gesture { get; }

        public const string MOVEMENT = "movement";
        public Vector2 Movement { get; }

        public ControllEvent(string name, Vector2 gesture) : base(name)
        {
            Gesture = gesture;
            Movement = gesture;
        }
    }
}