using AgkCommons.Event;
using BezierSolution;
using Drone.Location.Service.Control.Drone.Model;

namespace Drone.Location.Service.Game.Event
{
    public class InGameEvent : GameEvent
    {
        public const string SET_DRONE_PARAMETERS = "setDroneParameters";
        public const string START_GAME = "startGame";
        public const string END_GAME = "endGame";
        public const string CHANGE_SPLINE_SEGMENT = "changeRotation";
        public const string CHANGE_TILE = "changeTile";
        public const string RESPAWN = "respawn";

        public BezierSpline.Segment BezierSegment { get; private set; }
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
        public InGameEvent(string name, BezierSpline.Segment segment) : base(name)
        {
            BezierSegment = segment;
        }
    }
}