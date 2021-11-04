using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.World.Drone.Event
{
    public class ControllEvent : GameEvent
    {
        public const string GESTURE = "gesture";
        public Vector2 Gesture { get; }

        public ControllEvent(string name, Vector2 gesture) : base(name)
        {
            Gesture = gesture;
        }
    }
}