using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Finish;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.Finish
{
    public class FinishController : MonoBehaviour, IWorldObjectController<FinishModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(FinishModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.FINISHED));
            }
        }
    }
}