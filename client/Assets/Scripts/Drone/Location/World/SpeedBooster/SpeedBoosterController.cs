using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.SpeedBooster;
using Drone.World;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service.Control.SpeedBooster
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
            if (objectType == WorldObjectType.PLAYER) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new AcceleratorEvent(AcceleratorEvent.PICKED));
            }
        }
    }
}