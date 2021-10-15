using AgkCommons.Event;
using UnityEngine;

namespace Drone.Location.World.Drone.Event
{
    public class ControllEvent : GameEvent
    {
        public const string START_MOVE = "start";
        public const string END_MOVE = "end";
        public Vector2 Swipe { get; private set; }

        public ControllEvent(string name, Vector2 swipe) : base(name)
        {
            Swipe = swipe;
        }
        
    }
}