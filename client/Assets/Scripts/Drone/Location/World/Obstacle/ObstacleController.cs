using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Obstacle;
using Drone.World;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Obstacle
{
    public class ObstacleController : MonoBehaviour, IWorldObjectController<ObstacleModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(ObstacleModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType != WorldObjectType.DRON) {
                return;
            }
            ContactPoint[] contactPoints = otherCollision.contacts;
            _gameWorld.Require().Dispatch(new ObstacleEvent(ObstacleEvent.OBSTACLE_CONTACT, contactPoints, ImmersionDepth(otherCollision)));
        }

        private float ImmersionDepth(Collision otherCollision)
        {
            Physics.ComputePenetration(gameObject.GetComponent<Collider>(), transform.position, transform.rotation, otherCollision.collider,
                                       otherCollision.transform.position, otherCollision.transform.rotation, out Vector3 direction,
                                       out float distance);
            return distance;
        }
    }
}