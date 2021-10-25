using Drone.Descriptor;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.ShieldBooster;
using Drone.Location.Service;
using Drone.Location.World.Drone;
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
        private BoosterService _boosterService;
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
                if (_descriptor == null) {
                    _descriptor = _boosterService.GetDescriptorByType(ObjectType);
                }
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.ENABLE_SHIELD, _descriptor));
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.PLAY_ANIMATE, DroneAnimState.amEnableShield));
                Invoke(nameof(DisableBoost), float.Parse(_descriptor.Params["Duration"]));
            }
        }
        
        private void DisableBoost()
        {
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DISABLE_SHIELD, _descriptor));
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.PLAY_ANIMATE, DroneAnimState.amDisableShield));
        }
    }
}