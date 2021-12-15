using System.Collections.Generic;
using Drone.Location.Event;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service.Control.Battery
{
    public class BatteryController : MonoBehaviour, IWorldObjectController<BatteryModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(BatteryModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.PLAYER) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new EnergyEvent(EnergyEvent.PICKED));
            }
        }
    }
}