using System.Collections;
using AgkCommons.Event;
using Drone.Core.Service;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Battery;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service
{
    public class BatteryService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private GameService _gameService;

        private DroneModel _droneModel;

        private bool _isPlay;
        private Coroutine _fallingEnergy;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.START_FLIGHT, worldEvent => Start());
            _droneModel = _gameService.DroneModel;
        }

        private void Start()
        {
            _isPlay = true;
            _fallingEnergy = StartCoroutine(FallEnergy());
        }

        private void OnDronCollision(WorldEvent worldEvent) //todo выделить все в отдельные сервисы
        {
            Collision collisionObject = worldEvent.CollisionObject;
            switch (collisionObject.gameObject.GetComponent<PrefabModel>().ObjectType) {
                case WorldObjectType.BATTERY:
                    OnTakeBattery(collisionObject.gameObject.GetComponent<BatteryModel>());
                    break;
            }
        }

        private void OnTakeBattery(BatteryModel component)
        {
            _droneModel.energy += component.Energy;
        }

        private IEnumerator FallEnergy()
        {
            while (_isPlay) {
                _droneModel.energy -= _droneModel.energyFall;
                if (_droneModel.energy <= 0) {
                    _droneModel.energy = 0;
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_FAILED, FailedReasons.EnergyFalled));
                    StopCoroutine(_fallingEnergy);
                } else {
                    _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
}