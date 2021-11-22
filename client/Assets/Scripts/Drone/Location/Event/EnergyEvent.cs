using AgkCommons.Event;

namespace Drone.Location.Event
{
    public class EnergyEvent : GameEvent
    {
        public const string ENERGY_FALL = "energyFall";
        public const string ENERGY_UPDATE = "energyUpdate";
        public const string PICKED = "picked";

        public float EnergyValue { get; private set; }

        public EnergyEvent(string name, float energyValue) : base(name)
        {
            EnergyValue = energyValue;
        }

        public EnergyEvent(string name) : base(name)
        {
        }
    }
}