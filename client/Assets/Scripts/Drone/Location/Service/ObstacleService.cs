using AgkCommons.Event;
using Drone.Core.Service;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;

namespace Drone.Location.Service
{
    public class ObstacleService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private IoCProvider<BoosterService> _boosterService;

        private const float MAX_DISTANCE_DEPTH_COLLIDER = 0.0315f;

        private float damage = 3f;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.OBSTACLE_COLLISION, OnCollision);
        }

        private void OnCollision(WorldEvent worldEvent)
        {
            if (_boosterService.Require().IsShieldActivate) {
                return;
            }
            if (worldEvent.ImmersionDepth > MAX_DISTANCE_DEPTH_COLLIDER) {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_LETHAL_CRASH)); // todo определиться нужны ли летальные столкновения
                //_gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASH, worldEvent.ContactPoints, worldEvent.ImmersionDepth, damage));
            } else {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_CRASH, worldEvent.ContactPoints, worldEvent.ImmersionDepth, damage));
            }
        }
    }
}