using AgkCommons.Event;

namespace Drone.Location.Service.Game.Event
{
    public class DurabilityEvent : GameEvent
    {
        public const string UPDATED = "Updated";

        public float DurabilityValue { get; private set; }

        public DurabilityEvent(string name, float durability) : base(name)
        {
            DurabilityValue = durability;
        }
    }
}