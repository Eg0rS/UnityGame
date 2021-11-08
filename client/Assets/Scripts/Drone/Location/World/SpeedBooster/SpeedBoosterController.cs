using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.SpeedBooster;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.SpeedBooster
{
    public class SpeedBoosterController : MonoBehaviour, IWorldObjectController<SpeedBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(SpeedBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_SPEED));
            }
        }
    }
}