using AgkCommons.Event;
using Drone.Location.World.CutScene;
using Drone.Location.World.Player.Model;

namespace Drone.Location.Service.Game.Event
{
    public class InGameEvent : GameEvent
    {
        public const string START_GAME = "startGame";
        public const string END_GAME = "endGame";
        public const string CHIP_UP = "chipUp";
        public const string CHANGE_TILE = "changeTile";
        public const string RESPAWN = "respawn";
        public const string CUTSCENE_BEGIN = "cutSceneBegin";
        public const string CUTSCENE_END = "cutSceneEnd";

        public EndGameReasons EndGameReason { get; private set; }
        public DroneModel DroneModel { get; private set; }
        
        public CutSceneType CutSceneType { get; private set; }

        public InGameEvent(string name) : base(name)
        {
        }
        public InGameEvent(string name, DroneModel droneModel) : base(name)
        {
            DroneModel = droneModel;
        }
        
        public InGameEvent(string name, CutSceneType type) : base(name)
        {
            CutSceneType = type;
        }
        public InGameEvent(string name, EndGameReasons endGameReason) : base(name)
        {
            EndGameReason = endGameReason;
        }
    }
}