using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Magnet;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Magnet
{
    public class MagnetBoosterController : MonoBehaviour, IWorldObjectController<MagnetBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(MagnetBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_MAGNET, otherCollision.gameObject));
            }
        }
    }
}