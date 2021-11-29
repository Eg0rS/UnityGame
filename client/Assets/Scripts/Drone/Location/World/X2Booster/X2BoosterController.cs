using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.X2Booster;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.X2Booster
{
    public class X2BoosterController : MonoBehaviour, IWorldObjectController<X2BoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(X2BoosterModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new WorldObjectEvent(WorldObjectEvent.TAKE_X2));
            }
        }
    }
}