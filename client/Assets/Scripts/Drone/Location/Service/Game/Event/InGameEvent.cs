using AgkCommons.Event;
using Drone.Location.World.Drone.Model;

namespace Drone.Location.Service.Game.Event
{
    public class InGameEvent : GameEvent
    {
        public const string SET_DRONE_PARAMETERS = "setDroneParameters";
        public const string START_GAME = "startGame";
        public const string END_GAME = "endGame";

        public EndGameReasons EndGameReason { get; private set; }
        public DroneModel DroneModel { get; private set; }

        public InGameEvent(string name) : base(name)
        {
        }

        public InGameEvent(string name, DroneModel droneModel) : base(name)
        {
            DroneModel = droneModel;
        }

        public InGameEvent(string name, EndGameReasons endGameReason) : base(name)
        {
            EndGameReason = endGameReason;
        }
    }
}