using AgkCommons.Event;

namespace Drone.Location.Event
{
    public class EnergyEvent : GameEvent
    {
        public const string UPDATE = "Update";
        public const string PICKED = "picked";
        public const string CHANGED = "chaned";

        public float EnergyValue { get; private set; }
        public float EnergyDelta { get; private set; }

        public EnergyEvent(string name, float energyValue) : base(name)
        {
            EnergyValue = energyValue;
            EnergyDelta = energyValue;
        }
        
        public EnergyEvent(string name) : base(name)
        {
        }
    }
}