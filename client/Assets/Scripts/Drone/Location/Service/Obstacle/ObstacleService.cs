using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Location.Service.Obstacle
{
    public class ObstacleService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float DAMAGE = 3.0f;
        private const float MAX_DISTANCE_DEPTH_COLLIDER = 0.029f;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;
        private float _durability;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.SET_DRON_PARAMETERS, OnSetParameters);
            _gameWorld.Require().AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT, OnObstacleContact);
        }

        private void OnObstacleContact(ObstacleEvent obstacleEvent)
        {
            if (obstacleEvent.ImmersionDepth > MAX_DISTANCE_DEPTH_COLLIDER) {
                _gameWorld.Require().Dispatch(new ObstacleEvent(ObstacleEvent.LETHAL_CRUSH));
            } else {
                _durability -= DAMAGE;
                if (_durability <= 0) {
                    _durability = 0;
                    _gameWorld.Require().Dispatch(new ObstacleEvent(ObstacleEvent.LETHAL_CRUSH));
                } else {
                    _gameWorld.Require().Dispatch(new ObstacleEvent(ObstacleEvent.CRUSH, obstacleEvent.ContactPoints, obstacleEvent.ImmersionDepth));
                }
            }
            _gameWorld.Require().Dispatch(new ObstacleEvent(ObstacleEvent.DURABILITY_UPDATED, _durability));
        }

        private void OnSetParameters(WorldEvent worldEvent)
        {
            _durability = worldEvent.DroneModel.durability;
        }
    }
}