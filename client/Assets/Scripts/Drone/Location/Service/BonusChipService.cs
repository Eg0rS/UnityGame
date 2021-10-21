using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.BonusChips;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service
{
    public class BonusChipService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private GameService _gameService;

        private DroneModel _droneModel;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _droneModel = _gameService.DroneModel;
        }

        private void OnDronCollision(WorldEvent worldEvent) 
        {
            Collision collisionObject = worldEvent.CollisionObject;
            switch (collisionObject.gameObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.BONUS_CHIPS:
                    OnTakeChip(collisionObject.gameObject.GetComponent<BonusChipsModel>());
                    break;
            }
        }

        private void OnTakeChip(BonusChipsModel component)
        {
            _droneModel.countChips++;
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }
    }
}