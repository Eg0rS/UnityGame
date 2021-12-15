using AgkCommons.Event;
using Drone.Location.Service.Accelerator;
using Drone.Location.Service.Control.SpeedBooster;

namespace Drone.Location.Event
{
    public class AcceleratorEvent : GameEvent
    {
        public const string PICKED = "picked";
        public const string ACCELERATION = "acceleration";

        public AcceleratorModel AcceleratorModel { get; private set; }
        public SpeedBoosterController AccelerationController { get; private set; }

        public AcceleratorEvent(string name, AcceleratorModel model) : base(name)
        {
            AcceleratorModel = model;
        }

        public AcceleratorEvent(string name, SpeedBoosterController accelerationController) : base(name)
        {
            AccelerationController = accelerationController;
        }

        public AcceleratorEvent(string name) : base(name)
        {
            
        }
    }
}