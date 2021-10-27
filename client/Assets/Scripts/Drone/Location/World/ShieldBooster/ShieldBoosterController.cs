﻿using Drone.Descriptor;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.ShieldBooster;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.World.ShieldBooster
{
    public class ShieldBoosterController : MonoBehaviour, IWorldObjectController<ShieldBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }
        private BoosterDescriptor _descriptor;
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        public void Init(ShieldBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }

        private void OnCollisionEnter(Collision otherCollision)
        {
            WorldObjectType objectType = otherCollision.gameObject.GetComponent<PrefabModel>().ObjectType;
            if (objectType == WorldObjectType.DRON) {
                gameObject.SetActive(false);
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.TAKE_SHIELD));
            }
        }
    }
}