using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.World;
using IoC.Attribute;

namespace Drone.Location.Service.Obstacle
{
    public class ObstacleService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float DAMAGE = -3.0f;
        private const float MAX_DISTANCE_DEPTH_COLLIDER = 0.029f;
        [Inject]
        private DroneWorld _gameWorld;

        public void Init()
        {
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT, OnObstacleContact);
        }

        private void OnObstacleContact(ObstacleEvent obstacleEvent)
        {
            bool isLethalCrush = obstacleEvent.ImmersionDepth > MAX_DISTANCE_DEPTH_COLLIDER;
            _gameWorld.Dispatch(new ObstacleEvent(ObstacleEvent.CRUSH, isLethalCrush, DAMAGE));
        }
    }
}