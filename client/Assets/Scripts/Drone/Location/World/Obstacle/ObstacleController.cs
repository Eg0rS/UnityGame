using System;
using Adept.Logger;
using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Obstacle;
using IoC.Attribute;
using IoC.Extension;
using UnityEngine;

namespace Drone.Location.World.Obstacle
{
    public class ObstacleController : MonoBehaviour, IWorldObjectController<ObstacleModel>
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ObstacleController>();
        public WorldObjectType ObjectType { get; private set; }

        [Inject]
        private DroneWorld _gameWorld;
        [InjectComponent]
        private Rigidbody _rb;

        public void Init(ObstacleModel model)
        {
            this.InjectComponents();
            _rb.WakeUp();
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            Debug.Log(1);
            WorldObjectType objectType = otherCollision.gameObject.GetComponentInParent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.PLAYER) {
                _logger.Warn("Collider non-player contact.");
                return;
            }
            Debug.Log(Time.time);
            _gameWorld.Dispatch(new ObstacleEvent(ObstacleEvent.OBSTACLE_CONTACT_BEGIN));
        }

        private void OnCollisionExit(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.PLAYER) {
                _logger.Warn("Exit collider non-player contact.");
                return;
            }
            Debug.Log(Time.time);
            _gameWorld.Dispatch(new ObstacleEvent(ObstacleEvent.OBSTACLE_CONTACT_END));
        }
    }
}