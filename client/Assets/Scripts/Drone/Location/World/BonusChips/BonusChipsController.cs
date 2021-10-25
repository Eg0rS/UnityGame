using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.BonusChips;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.BonusChips
{
    public class BonusChipsController : MonoBehaviour, IWorldObjectController<BonusChipsModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(BonusChipsModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_CHIP));
            }
        }
    }
}