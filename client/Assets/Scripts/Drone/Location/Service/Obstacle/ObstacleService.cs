using AgkCommons.Event;
using DG.Tweening;
using Drone.Core.Service;
using Drone.Location.Event;
using Drone.Location.World;
using IoC.Attribute;

namespace Drone.Location.Service.Obstacle
{
    public class ObstacleService : GameEventDispatcher, IWorldServiceInitiable
    {
        private const float DEATH_TIME = 0.1F;
        private Tween _tween;
        [Inject]
        private DroneWorld _gameWorld;

        public void Init()
        {
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT_BEGIN, OnObstacleContactBegin);
            _gameWorld.AddListener<ObstacleEvent>(ObstacleEvent.OBSTACLE_CONTACT_END, OnObstacleContactEnd);
        }

        private void OnObstacleContactEnd(ObstacleEvent obj)
        {
            _tween.Kill();
        }

        private void OnObstacleContactBegin(ObstacleEvent obstacleEvent)
        {
            _tween = DOVirtual.DelayedCall(DEATH_TIME, () => _gameWorld.Dispatch(new ObstacleEvent(ObstacleEvent.CRUSH)), false);
        }
    }
}