using System.Collections.Generic;
using AgkCommons.Event;
using Cinemachine;
using Drone.Booster.Descriptor;
using Drone.Core.Service;
using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.Obstacle;
using Drone.Location.World.Drone;
using Drone.Location.World.Drone.Model;
using Drone.World;
using Drone.World.Event;
using IoC.Attribute;
using IoC.Util;
using UnityEngine;

namespace Drone.Location.Service
{
    public class ObstacleService : GameEventDispatcher, IWorldServiceInitiable
    {
        [Inject]
        private IoCProvider<GameWorld> _gameWorld;

        [Inject]
        private IoCProvider<DroneAnimService> _droneAnimService;

        [Inject]
        private GameService _gameService;

        private List<BoosterDescriptor> _boosterDescriptors;
        private CinemachineBasicMultiChannelPerlin _cameraNoise;
        private DroneModel _droneModel;

        private float _crashNoise = 2;
        private float _crashNoiseDuration = 0.5f;

        private bool _onActiveShield;

        public void Init()
        {
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ENABLE_SHIELD, EnableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.DISABLE_SHIELD, DisableShield);
            _gameWorld.Require().AddListener<WorldEvent>(WorldEvent.ON_COLLISION, OnDronCollision);
            _droneModel = _gameService.DroneModel;
            CinemachineVirtualCamera camera = _gameWorld.Require().GetDroneCamera();
            _cameraNoise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void EnableShield(WorldEvent obj)
        {
            _onActiveShield = true;
        }

        private void DisableShield(WorldEvent obj)
        {
            _onActiveShield = false;
        }

        private void OnDronCollision(WorldEvent worldEvent)
        {
            Collision collisionObject = worldEvent.CollisionObject;
            WorldObjectType objectType = collisionObject.gameObject.GetComponent<PrefabModel>().ObjectType;
            switch (objectType) {
                case WorldObjectType.OBSTACLE:
                    OnDronCrash(collisionObject.gameObject.GetComponent<ObstacleModel>(), collisionObject.contacts);
                    break;
            }
        }

        private void OnDronCrash(ObstacleModel component, ContactPoint[] contactPoints)
        {
            if (_onActiveShield) {
                return;
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.CRASH));
            _cameraNoise.m_AmplitudeGain += _crashNoise;
            foreach (ContactPoint contact in contactPoints) {
                _droneAnimService.Require().PlayParticleState(DroneParticles.ptSparks, contact.point, component.transform.rotation);
            }
            Invoke(nameof(DisableCrashNoise), _crashNoiseDuration);
            _droneModel.durability -= component.Damage;
            if (_droneModel.durability <= 0) {
                _droneModel.durability = 0;
                _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.DRONE_FAILED, FailedReasons.Crashed));
            }
            _gameWorld.Require().Dispatch(new WorldEvent(WorldEvent.UI_UPDATE, _droneModel));
        }

        private void DisableCrashNoise()
        {
            _cameraNoise.m_AmplitudeGain -= _crashNoise;
        }
    }
}