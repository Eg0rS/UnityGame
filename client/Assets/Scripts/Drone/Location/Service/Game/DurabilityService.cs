using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.Service.Game.Event;
using Drone.World;
using IoC.Attribute;

namespace Drone.Location.Service.Game
{
    public class DurabilityService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private GameWorld _gameWorld;
        private float _durability;

        public void Init()
        {
            _gameWorld.AddListener<InGameEvent>(InGameEvent.SET_DRONE_PARAMETERS, OnSetParameters);
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.CRUSH, OnCrush);
        }

        private void OnCrush(ObstacleEvent obstacleEvent)
        {
            if (obstacleEvent.IsLethalCrush) {
                _gameWorld.Dispatch(new InGameEvent(InGameEvent.END_GAME, EndGameReasons.OUT_OF_DURABILITY));
                return;
            }
            _durability += obstacleEvent.DurabilityDelta;
            if (_durability <= 0) {
                _durability = 0;
                _gameWorld.Dispatch(new InGameEvent(InGameEvent.END_GAME, EndGameReasons.OUT_OF_DURABILITY));
            }
            _gameWorld.Dispatch(new DurabilityEvent(DurabilityEvent.UPDATED, _durability));
        }

        private void OnSetParameters(InGameEvent inGameEvent)
        {
            _durability = inGameEvent.DroneModel.durability;
        }
    }
}